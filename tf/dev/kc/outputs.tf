output "kc_clients" {
  description = "Map of created Keycloak clients with Key Vault secret references"
  value       = module.kc_setup.clients
}

output "kc_client_ids" {
  description = "Map of client keys to their Keycloak client IDs"
  value       = module.kc_setup.client_ids
}

output "kc_roles" {
  description = "Map of all created roles by client_role key"
  value       = module.kc_setup.roles
}

output "kc_roles_by_client" {
  description = "Nested map of roles organized by client"
  value       = module.kc_setup.roles_by_client
}

output "kc_groups" {
  description = "Map of created Keycloak groups"
  value       = module.kc_setup.groups
}

output "kc_group_ids" {
  description = "Map of group keys to their Keycloak group IDs"
  value       = module.kc_setup.group_ids
}

output "authority_base" {
  value = module.kc.terraform_client.url
}
