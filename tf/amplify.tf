# resource "aws_amplify_app" "tca_app" {
#   name                     = "${local.app_name_with_env}"
#   repository               = var.tca_existing_repo_url
#   enable_branch_auto_build = true

#   # OPTIONAL - Necessary if not using oauth_token or access_token (used for GitLab and GitHub repos)
#   iam_service_role_arn = aws_iam_role.tca_amplify_codecommit.arn
#   access_token         = var.lookup_ssm_github_access_token ? data.aws_ssm_parameter.ssm_github_access_token[0].name : var.github_access_token // optional, only needed if using github repo

#   build_spec = file("${path.root}/../amplify.yml")
#   # Redirects for Single Page Web Apps (SPA)
#   # https://docs.aws.amazon.com/amplify/latest/userguide/redirects.html#redirects-for-single-page-web-apps-spa
#   custom_rule {
#     source = "</^[^.]+$|\\.(?!(css|gif|ico|jpg|js|png|txt|svg|woff|ttf|map|json)$)([^.]+$)/>"
#     status = "200"
#     target = "/index.html"
#   }

#   environment_variables = {
#     TCA_REGION              = "${data.aws_region.current.id}"
#     TCA_CODECOMMIT_REPO_ID  = "${aws_codecommit_repository.tca_codecommit_repo[0].repository_id}"
#     TCA_USER_POOL_ID        = "${aws_cognito_user_pool.tca_user_pool.id}"
#     TCA_IDENTITY_POOL_ID    = "${aws_cognito_identity_pool.tca_identity_pool.id}"
#     TCA_APP_CLIENT_ID       = "${aws_cognito_user_pool_client.tca_user_pool_client.id}"
#     TCA_GRAPHQL_ENDPOINT    = "${aws_appsync_graphql_api.tca_appsync_graphql_api.uris.GRAPHQL}"
#     TCA_GRAPHQL_API_ID      = "${aws_appsync_graphql_api.tca_appsync_graphql_api.id}"
#     TCA_LANDING_BUCKET_NAME = "${aws_s3_bucket.tca_landing_bucket.id}"
#   }
# }
