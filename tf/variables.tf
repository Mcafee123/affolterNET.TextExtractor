# General Variables
variable "environment" {
  description = "The environment in which the resources are deployed"
  type        = string
  default     = "dev"
}
variable "aws_region" {
  type    = string
  default = "eu-central-1"
}

variable "tags" {
  type        = map(any)
  description = "Tags to apply to resources"
  default = {
    IAC_PROVIDER = "Terraform"
  }
}
variable "app_name" {
  type    = string
  default = "textextractor"
}

# - Cognito -
# Admin Users to create
variable "cog_admin_cognito_users" {
  type = map(object({
    username       = string
    given_name     = string
    family_name    = string
    email          = string
    email_verified = bool
  }))
  default = {
    MartinAffolter : {
      username       = "mcafee"
      given_name     = "Martin"
      family_name    = "Affolter"
      email          = "martin@affolter.net"
      email_verified = true // no touchy
    },
  }
}
# Standard Users to create
variable "cog_standard_cognito_users" {
  type = map(object({
    username       = string
    given_name     = string
    family_name    = string
    email          = string
    email_verified = bool
  }))
  default = {
    DefaultStandardUser : {
      username       = "default"
      given_name     = "Default"
      family_name    = "User"
      email          = "example@example.com"
      email_verified = true // no touchy
    }
  }
}
