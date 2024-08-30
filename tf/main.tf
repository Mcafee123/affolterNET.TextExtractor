resource "aws_resourcegroups_group" "textextractor" {
  name        = local.app_name_with_env
  description = "Resource group for the textextractor service"
  resource_query {
    query = jsonencode({
      "ResourceTypeFilters" : ["AWS::AllSupported"],
      "TagFilters" : [
        {
          "Key" : "AppName",
          "Values" : ["textextractor"]
        }
      ]
    })
  }
}

