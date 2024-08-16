#!/bin/bash

# author: martin@affolter.net

# application insights: az extension add -n application-insights
# brew tap azure/functions
# brew install azure-functions-core-tools@4

# DEPLOY INFRASTRUCTURE FOR EXT APP
set -e

env=${1:-prod}

. _config.sh $env

# az_login
set_subscription

# create the resource group
az group create -n "$resourceGroup" -l "$location"

# to see if anything was already in there
az resource list -g $resourceGroup -o tsv

# create a storage account
az storage account create \
  -n "$storageAccountName" \
  -l "$location" \
  -g "$resourceGroup" \
  --sku Standard_LRS

storageAccountKey=$(az storage account keys list -n $storageAccountName --query [0].value -o tsv)

echo
echo "create __${env}_stor.sh"
echo "export STORAGE_ACCOUNT_NAME=\"$storageAccountName\"" > __${env}_stor.sh
echo "export STORAGE_ACCOUNT_KEY"=\"$storageAccountKey\" >> __${env}_stor.sh
echo

# create log analytics workspace
az monitor log-analytics workspace create \
  -g "$resourceGroup" \
  -n "$logAnalyticsWorkspace"

loganalyticsworkspacekey=$(az monitor log-analytics workspace get-shared-keys -n "$logAnalyticsWorkspace" -g "$resourceGroup" --query primarySharedKey -o tsv)
echo
echo "create __${env}_law.sh"
echo "export LOG_ANALYTICS_WORKSPACE_KEY=\"$loganalyticsworkspacekey\"" > __${env}_law.sh
echo

# # create an app insights instance
# appInsightsName="$basename-insights"
# instrumentationKey=$(az monitor app-insights component create \
#   --app "$appInsightsName" \
#   --location "$location" \
#   -g "$resourceGroup" \
#   --workspace "$logAnalyticsWorkspace" \
#   --kind web \
#   --application-type web \
#   --query instrumentationKey \
#   -o tsv)

# echo
# echo "create __${env}_insights.sh"
# echo "export APPINSIGHTS_INSTRUMENTATIONKEY=\"$instrumentationKey\"" > __${env}_insights.sh
# echo

# swa (GITHUB-INTEGRATION)
# url="$(az staticwebapp create \
#   --branch "$branch_name" \
#   --location "$location" \
#   --name "$swaAppName" \
#   --resource-group "$resourceGroup" \
#   --source "$static_site_repo" \
#   --sku Free \
#   --login-with-github \
#   --query defaultHostname)"

url="$(az staticwebapp create \
  --location "$location" \
  --name "$swaAppName" \
  --resource-group "$resourceGroup" \
  --sku Free \
  --query defaultHostname)"

echo "url: $url"
echo "echo \"create CNAME entry '$application_url' for $url\"" > __url.sh

# add settings
# insights="APPINSIGHTS_INSTRUMENTATIONKEY=$instrumentationKey"
# insightsConn="APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=$instrumentationKey"
stor="EXTSTOR=DefaultEndpointsProtocol=https;AccountName=$storageAccountName;AccountKey=$storageAccountKey;EndpointSuffix=core.windows.net"
customDomain="SpaUrl=https://ext.affolter.net"

az staticwebapp appsettings set \
  -n "$swaAppName" \
  --setting-names \
    "$stor" \
    "$customDomain"
