settings = {
  init_github    = true
  az_login_name  = "5700c6f1-4c18-44db-962d-a7d437d1b424"
  scripts_folder = "../../scripts"
  variable_name  = "platform"
  base_name      = "text-e"
  github_owner   = "affolterNET"
  github_repo    = "affolterNET.TextExtractor"
  modules = [
    {
      module      = "app"
      environment = "dev"
      path        = "../"
    },
    {
      module      = "kc"
      environment = "dev"
      path        = "../"
    },
    {
      module      = "app"
      environment = "prod"
      path        = "../"
    },
    {
      module      = "kc"
      environment = "prod"
      path        = "../"
    }    
  ]
}
