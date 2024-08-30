# --- IAM ROLES ---
# - Cognito Roles -
# Cognito AuthRole Restricted Access
# Role granting restricted access permissions to Cognito authenticated users
resource "aws_iam_role" "textextractor_cognito_authrole_restricted_access" {
  # Conditional create of the role - default is 'TRUE'
  count = var.create_restricted_access_roles ? 1 : 0

  name               = "textextractor_authRole_restricted_access"
  assume_role_policy = data.aws_iam_policy_document.textextractor_cognito_authrole_trust_relationship.json
  managed_policy_arns = [
    "arn:aws:iam::aws:policy/CloudWatchLogsFullAccess",
    # aws_iam_policy.textextractor_s3_restricted_access_policy[0].arn,
    # aws_iam_policy.textextractor_ssm_restricted_access_policy[0].arn
  ]

  force_detach_policies = true
  path                  = "/${var.app_name}/"
  tags = merge(
    {
      "AppName" = var.app_name
    },
    var.tags,
  )
}
# Cognito UnAuth Role
# Role granting restricted access permissions to Cognito unauthenticated users
resource "aws_iam_role" "textextractor_cognito_unauthrole_restricted_access" {
  # Conditional create of the role - default is 'TRUE'
  count = var.create_restricted_access_roles ? 1 : 0

  name               = "textextractor_unauthRole_restricted_access"
  assume_role_policy = data.aws_iam_policy_document.textextractor_cognito_unauthrole_trust_relationship.json

  # Managed Policies
  managed_policy_arns = [
    "arn:aws:iam::aws:policy/AmazonS3ReadOnlyAccess",
  ]

  force_detach_policies = true
  path                  = "/${var.app_name}/"
  tags = merge(
    {
      "AppName" = var.app_name
    },
    var.tags,
  )
}
# Cognito Admin Group Role (Restricted Access)
resource "aws_iam_role" "textextractor_cognito_admin_group_restricted_access" {
  # Conditional create of the role - default is 'TRUE'
  count = var.create_restricted_access_roles ? 1 : 0

  name               = "textextractor_cognito_admin_group_restricted_access"
  assume_role_policy = data.aws_iam_policy_document.textextractor_cognito_admin_group_trust_relationship.json
  description        = "Role granting full DynamoDB permissions for the textextractor_outputs DynamoDB table."
  managed_policy_arns = [
    "arn:aws:iam::aws:policy/CloudWatchLogsFullAccess",
    # aws_iam_policy.textextractor_s3_restricted_access_policy[0].arn,
    # aws_iam_policy.textextractor_dynamodb_restricted_access_policy[0].arn
  ]

  force_detach_policies = true
  path                  = "/${var.app_name}/"
  tags = merge(
    {
      "AppName" = var.app_name
    },
    var.tags,
  )
}
# Cognito Standard Group Role (Restricted Access)
resource "aws_iam_role" "textextractor_cognito_standard_group_restricted_access" {
  # Conditional create of the role - default is 'TRUE'
  count = var.create_restricted_access_roles ? 1 : 0

  name               = "textextractor_cognito_standard_group_restricted_access"
  assume_role_policy = data.aws_iam_policy_document.textextractor_cognito_standard_group_trust_relationship.json
  description        = "Role granting restricted (read-only) DynamoDB permissions for the textextractor_outputs DynamoDB table."
  managed_policy_arns = [
    "arn:aws:iam::aws:policy/CloudWatchLogsFullAccess",
    # aws_iam_policy.textextractor_s3_restricted_access_policy[0].arn,
    # aws_iam_policy.textextractor_dynamodb_restricted_access_read_only_policy[0].arn
  ]

  force_detach_policies = true
  path                  = "/${var.app_name}/"
  tags = merge(
    {
      "AppName" = var.app_name
    },
    var.tags,
  )
}

# --- TRUST RELATIONSHIPS ---
# Cognito Trust Relationship (AuthRole)
data "aws_iam_policy_document" "textextractor_cognito_authrole_trust_relationship" {
  statement {
    effect  = "Allow"
    actions = ["sts:AssumeRoleWithWebIdentity"]
    principals {
      type        = "Federated"
      identifiers = ["cognito-identity.amazonaws.com"]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:aud"
      values   = [aws_cognito_identity_pool.textextractor_identity_pool.id]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:amr"
      values   = ["authenticated"]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:aud"
      values   = [aws_cognito_identity_pool.textextractor_identity_pool.id]
    }
  }
}
# Cognito Trust Relationship (UnauthRole)
data "aws_iam_policy_document" "textextractor_cognito_unauthrole_trust_relationship" {
  statement {
    effect  = "Allow"
    actions = ["sts:AssumeRoleWithWebIdentity"]
    principals {
      type        = "Federated"
      identifiers = ["cognito-identity.amazonaws.com"]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:aud"
      values   = [aws_cognito_identity_pool.textextractor_identity_pool.id]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:amr"
      values   = ["unauthenticated"]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:aud"
      values   = [aws_cognito_identity_pool.textextractor_identity_pool.id]
    }
  }
}
# Cognito Admin Group Trust Relationship
data "aws_iam_policy_document" "textextractor_cognito_admin_group_trust_relationship" {
  statement {
    effect  = "Allow"
    actions = ["sts:AssumeRoleWithWebIdentity"]
    principals {
      type        = "Federated"
      identifiers = ["cognito-identity.amazonaws.com"]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:aud"
      values   = [aws_cognito_identity_pool.textextractor_identity_pool.id]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:amr"
      values   = ["authenticated"]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:aud"
      values   = [aws_cognito_identity_pool.textextractor_identity_pool.id]
    }
  }
}
# Cognito Standard Group Trust Relationship
data "aws_iam_policy_document" "textextractor_cognito_standard_group_trust_relationship" {
  statement {
    effect  = "Allow"
    actions = ["sts:AssumeRoleWithWebIdentity"]
    principals {
      type        = "Federated"
      identifiers = ["cognito-identity.amazonaws.com"]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:aud"
      values   = [aws_cognito_identity_pool.textextractor_identity_pool.id]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:amr"
      values   = ["authenticated"]
    }
    condition {
      test     = "ForAnyValue:StringLike"
      variable = "cognito-identity.amazonaws.com:aud"
      values   = [aws_cognito_identity_pool.textextractor_identity_pool.id]
    }
  }
}
