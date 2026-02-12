variable "settings" {
  type = object({
    init_github    = bool
    az_login_name  = string
    scripts_folder = string
    variable_name  = string
    base_name      = string
    github_owner   = string
    github_repo    = string
    modules = list(object({
      module      = string
      environment = string
      path        = string
      base_name   = optional(string)
    }))
  })
}

locals {
  modules = [
    for m in var.settings.modules : merge(m, {
      base_name = coalesce(m.base_name, var.settings.base_name)
    })
  ]
}

module "statefiles_generator" {
  source = "git@github.com:affolterNET/affolterNET-Cloud-HelperModules.git//statefiles-generator?ref=main"
  basics = {
    subscription_id = "93a208c4-9c58-4f97-b35f-9beb52df1041"
    tenant_id       = "9c0f6304-c41a-4891-8379-ed3cbfc54535"
    location        = "switzerlandnorth"
  }
  az_login_name  = var.settings.az_login_name
  scripts_folder = var.settings.scripts_folder
  variable_name  = var.settings.variable_name
  modules        = local.modules

  state_rg        = "an_platform_state_rg"
  state_storage   = "anplatformstate"
  state_container = "tfstate"

  sp_login_settings = {
    keyvault_name                = "affolternet-vault"
    sp_client_id_secret_name     = "an-platform-state-$${env}-sp-client-id"
    sp_client_secret_secret_name = "an-platform-state-$${env}-sp-client-secret"
  }
}

# Generate backend.tf
resource "local_file" "backend" {
  for_each = module.statefiles_generator.state_config.modules_map
  filename = "${each.value.backend_path}/_basics.tf"
  content = templatefile("_basics.tf.tpl", {
    base_name   = var.settings.base_name
    environment = each.value.environment
  })
  file_permission = "0644"
}

# run github_init.sh to set github secrets
resource "null_resource" "github_init" {
  count = var.settings.init_github ? 1 : 0
  provisioner "local-exec" {
    command = "${path.module}/_github_init.sh ${var.settings.base_name} ${var.settings.github_owner} ${var.settings.github_repo}"
  }
}
