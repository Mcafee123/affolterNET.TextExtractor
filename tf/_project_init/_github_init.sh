#!/bin/bash

# author: martin@affolter.net

yellow() { printf "\e[33m$*\e[0m\n"; }
green() { printf  "\e[32m$*\e[0m\n"; }
red() { printf  "\e[31m$*\e[0m\n"; }
blue() { printf "\e[34m$*\e[0m\n"; }

# Helper functions for logging (output to stderr)
log_info() {
    blue "ℹ️  $1" >&2
}

log_success() {
    green "✅ $1" >&2
}

log_warning() {
    yellow "⚠️  $1" >&2
}

log_error() {
    red "❌ $1" >&2
}

get_kv_secret() {
  local kv_name="$1"
  local secret_name="$2"

  secret_value=$(az keyvault secret show --vault-name "$kv_name" --name "$secret_name" --query "value" -o tsv 2>/dev/null)
  # Remove any trailing whitespace/newlines
  secret_value=$(echo "$secret_value" | tr -d '\n\r' | sed 's/[[:space:]]*$//')
  echo "$secret_value"
}

# Set the GitHub secret
set_github_secret() {
    local secret_name="$1"
    local secret_value="$2"
    local github_owner="$3"
    local main_repo="$4"
    
    log_info "Setting GitHub repository secret '$secret_name'..."
    
    # Check if secret_value is provided
    if [ -z "$secret_value" ]; then
        log_error "Secret value is empty for '$secret_name'"
        return 1
    fi
    
    # Set the GitHub secret using the provided value
    if echo "$secret_value" | gh secret set "$secret_name" --repo "${github_owner}/${main_repo}"; then
        log_success "GitHub secret '$secret_name' has been set successfully"
        return 0
    else
        log_error "Failed to set GitHub secret '$secret_name'"
        return 1
    fi
}


. _config.sh

PROJECT="$1"
GITHUB_OWNER="$2"
MAIN_REPO="$3"

blue "PROJECT: $PROJECT"
blue "GITHUB_OWNER: $GITHUB_OWNER"
blue "MAIN_REPO: $MAIN_REPO"
blue "ACR Name: $ACR_NAME"
blue "ACR Username: $ACR_USERNAME"
blue "ACR Password Secret Name: $ACR_PW_SECRET_NAME"

ACR_PASSWORD=$(get_kv_secret "$KEYVAULT_NAME" "$ACR_PW_SECRET_NAME")

ARM_CLIENT_ID_DEV_SECRET_NAME="an-platform-state-dev-sp-client-id"
ARM_CLIENT_SECRET_DEV_SECRET_NAME="an-platform-state-dev-sp-client-secret"
blue "ARM Client ID DEV Secret Name: $ARM_CLIENT_ID_DEV_SECRET_NAME"
blue "ARM Client Secret DEV Secret Name: $ARM_CLIENT_SECRET_DEV_SECRET_NAME"
ARM_CLIENT_ID_DEV=$(get_kv_secret "$KEYVAULT_NAME" "$ARM_CLIENT_ID_DEV_SECRET_NAME")
ARM_CLIENT_SECRET_DEV=$(get_kv_secret "$KEYVAULT_NAME" "$ARM_CLIENT_SECRET_DEV_SECRET_NAME")

ARM_CLIENT_ID_PROD_SECRETNAME="an-platform-state-prod-sp-client-id"
ARM_CLIENT_SECRET_PROD_SECRET_NAME="an-platform-state-prod-sp-client-secret"
blue "ARM Client ID PROD Secret Name: $ARM_CLIENT_ID_PROD_SECRETNAME"
blue "ARM Client Secret PROD Secret Name: $ARM_CLIENT_SECRET_PROD_SECRET_NAME"
ARM_CLIENT_ID_PROD=$(get_kv_secret "$KEYVAULT_NAME" "$ARM_CLIENT_ID_PROD_SECRETNAME")
ARM_CLIENT_SECRET_PROD=$(get_kv_secret "$KEYVAULT_NAME" "$ARM_CLIENT_SECRET_PROD_SECRET_NAME")

# Set ACR secrets
set_github_secret "ACR_NAME" "$ACR_NAME" "$GITHUB_OWNER" "$MAIN_REPO"
set_github_secret "ACR_USERNAME" "$ACR_USERNAME" "$GITHUB_OWNER" "$MAIN_REPO"
set_github_secret "ACR_PASSWORD" "$ACR_PASSWORD" "$GITHUB_OWNER" "$MAIN_REPO"

# Set ARM secrets for Terraform
set_github_secret "ARM_CLIENT_ID" "$ARM_CLIENT_ID_PROD" "$GITHUB_OWNER" "$MAIN_REPO"
set_github_secret "ARM_CLIENT_SECRET" "$ARM_CLIENT_SECRET_PROD" "$GITHUB_OWNER" "$MAIN_REPO"
set_github_secret "ARM_TENANT_ID" "$TENANT_ID" "$GITHUB_OWNER" "$MAIN_REPO"
set_github_secret "ARM_SUBSCRIPTION_ID" "$SUBSCRIPTION_ID" "$GITHUB_OWNER" "$MAIN_REPO"

# Create AZURE_CREDENTIALS JSON for Azure CLI login
AZURE_CREDENTIALS=$(cat <<EOF
{
  "clientId": "$ARM_CLIENT_ID_PROD",
  "clientSecret": "$ARM_CLIENT_SECRET_PROD",
  "subscriptionId": "$SUBSCRIPTION_ID",
  "tenantId": "$TENANT_ID"
}
EOF
)

set_github_secret "AZURE_CREDENTIALS" "$AZURE_CREDENTIALS" "$GITHUB_OWNER" "$MAIN_REPO"
