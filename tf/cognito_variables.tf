# - Cognito -
variable "create_restricted_access_roles" {
  type        = bool
  default     = true
  description = "Conditional creation of TEXTEXTRACTOR restricted access roles"

}
# User Pool
variable "textextractor_user_pool_name" {
  type        = string
  default     = "textextractor_user_pool"
  description = "The name of the Cognito User Pool created"
}
variable "textextractor_user_pool_client_name" {
  type        = string
  default     = "textextractor_user_pool_client"
  description = "The name of the Cognito User Pool Client created"
}
variable "textextractor_identity_pool_name" {
  type        = string
  default     = "textextractor_identity_pool"
  description = "The name of the Cognito Identity Pool created"
}
variable "textextractor_identity_pool_allow_unauthenticated_identites" {
  type    = bool
  default = false
}
variable "textextractor_identity_pool_allow_classic_flow" {
  type    = bool
  default = false
}
variable "cog_invite_email_message" {
  type    = string
  default = <<-EOF
    You have been invited to the TCA App! Your username is "{username}" and
    temporary password is "{####}". Please reach out to an admin if you have issues signing in.

  EOF

}
# Invite E-Mail
variable "cog_invite_email_subject" {
  type    = string
  default = <<-EOF
  You've been CHOSEN.
  EOF

}
variable "cog_invite_sms_message" {
  type    = string
  default = <<-EOF
    You have been invited to the TCA App! Your username is "{username}" and
    temporary password is "{####}".

  EOF

}
# Password Policy
variable "cog_password_policy_min_length" {
  type        = number
  default     = 8
  description = "The minimum nmber of characters for Cognito user passwords"
}
variable "cog_password_policy_require_lowercase" {
  type        = bool
  default     = true
  description = "Whether or not the Cognito user password must have at least 1 lowercase character"
}
variable "cog_password_policy_require_uppercase" {
  type        = bool
  default     = true
  description = "Whether or not the Cognito user password must have at least 1 uppercase character"
}
variable "cog_password_policy_require_numbers" {
  type        = bool
  default     = true
  description = "Whether or not the Cognito user password must have at least 1 number"

}
variable "cog_password_policy_require_symbols" {
  type        = bool
  default     = true
  description = "Whether or not the Cognito user password must have at least 1 special character"
}
variable "cog_password_policy_temp_password_validity_days" {
  type        = number
  default     = 7
  description = "The number of days a temp password is valid. If user does not sign-in during this time, will need to be reset by an admin"
}
# General Schema
variable "cog_schemas" {
  description = "A container with the schema attributes of a user pool. Maximum of 50 attributes"
  type        = list(any)
  default     = []
}
# Schema (String)
variable "cog_string_schemas" {
  description = "A container with the string schema attributes of a user pool. Maximum of 50 attributes"
  type        = list(any)
  default = [{
    name                     = "email"
    attribute_data_type      = "String"
    required                 = true
    mutable                  = false
    developer_only_attribute = false

    string_attribute_constraints = {
      min_length = 7
      max_length = 25
    }
    },
    {
      name                     = "given_name"
      attribute_data_type      = "String"
      required                 = true
      mutable                  = true
      developer_only_attribute = false

      string_attribute_constraints = {
        min_length = 1
        max_length = 25
      }
    },
    {
      name                     = "family_name"
      attribute_data_type      = "String"
      required                 = true
      mutable                  = true
      developer_only_attribute = false

      string_attribute_constraints = {
        min_length = 1
        max_length = 25
      }
    },
    {
      name                     = "IAC_PROVIDER"
      attribute_data_type      = "String"
      required                 = false
      mutable                  = true
      developer_only_attribute = false

      string_attribute_constraints = {
        min_length = 1
        max_length = 10
      }
    },
  ]
}
# Schema (number)
variable "cog_number_schemas" {
  description = "A container with the number schema attributes of a user pool. Maximum of 50 attributes"
  type        = list(any)
  default     = []
}

# Admin Users
variable "cog_admin_cognito_user_group_name" {
  type    = string
  default = "Admin"
}
variable "cog_admin_cognito_user_group_description" {
  type    = string
  default = "Admin Group"
}
# Standard Users
variable "cog_standard_cognito_user_group_name" {
  type    = string
  default = "Standard"
}
variable "cog_standard_cognito_user_group_description" {
  type    = string
  default = "Standard Group"
}