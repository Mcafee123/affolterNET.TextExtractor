terraform {
  backend "remote" {
    organization = "affolterNET"
    workspaces {
      name = "textextractor"
    }
  }
}