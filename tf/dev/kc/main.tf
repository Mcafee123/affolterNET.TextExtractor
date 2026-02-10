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
  keyvault = {
    name    = var.platform.keyvault_name
    rg_name = var.platform.state_rg
  }

  realm = {
    name = module.kc.terraform_client.realm
  }
  clients = var.clients
}
