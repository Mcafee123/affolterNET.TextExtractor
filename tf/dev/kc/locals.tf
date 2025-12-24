locals {
  base_name_dashes  = replace(var.basics.base_name, "_", "-")
  base_name_letters = replace(var.basics.base_name, "_", "")
}
