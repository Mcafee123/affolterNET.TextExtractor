# ===== USERS =====

variable "users" {
  description = "Map of users with their username and group assignments (users must exist in the realm)"
  type = map(object({
    username = string                     # Keycloak username to lookup (user must already exist in realm)
    groups   = optional(list(string), []) # List of group keys to assign this user to
  }))
  default = {}
}

# ===== CLIENTS =====

variable "clients" {
  description = "Map of clients to create with their configuration"
  type = map(object({
    client_id    = string
    display_name = string
    host_names   = list(string)
    roles        = optional(list(string), [])
  }))
  default = {}
}

# ===== GROUPS =====

variable "groups" {
  description = "Map of groups to create with their configuration"
  type = map(object({
    name         = string
    description  = optional(string, "")
    client_roles = list(string) # Role names to assign from ALL clients (e.g., ["admin", "reader"])
  }))
  default = {}
}