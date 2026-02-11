module "kc" {
  source = "git@github.com:Mcafee123/affolterNET-Cloud-Keycloak.git//keycloak-tf-client?ref=kc_26_5_3"
  state = {
    state_rg        = var.platform.state_rg
    state_storage   = var.platform.state_storage
    state_container = var.platform.state_container
    state_map_key   = "platform"
    dest_key        = "${var.basics.environment}_core"
  }
}

# ===== CLIENT ONLY MODE - NO USERS NECESSARY =====
# All users in the realm will have access to the clients

module "kc_setup" {
  source = "git@github.com:Mcafee123/affolterNET-Cloud-Keycloak.git//kc_setup?ref=kc_26_5_3"

  realm_id = module.kc.terraform_client.realm
  clients  = var.clients
}

# ===== AZURE KEYVAULT SECRETS =====

data "azurerm_key_vault" "kv" {
  name                = var.platform.keyvault_name
  resource_group_name = var.platform.state_rg
}

resource "azurerm_key_vault_secret" "client_secrets" {
  for_each = var.clients

  name         = "${each.value.client_id}-client-secret"
  value        = module.kc_setup.clients[each.key].client_secret
  key_vault_id = data.azurerm_key_vault.kv.id
}
