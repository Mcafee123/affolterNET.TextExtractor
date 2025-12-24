terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.51.0"
    }
    local = {
      source  = "hashicorp/local"
      version = "~> 2.5.3"
    }
    keycloak = {
      source = "keycloak/keycloak"
      version = "5.4.0"
    }
  }
}

provider "azurerm" {
  subscription_id = var.platform.subscription_id
  features {}
}

provider "keycloak" {
  client_id     = module.kc.terraform_client.client_id
  client_secret = module.kc.terraform_client.client_secret
  url           = module.kc.terraform_client.url
}
