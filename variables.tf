variable "resource_group_name" {
  type        = string
  description = "The name of the resource group"
}

variable "subscription_id" {
  type        = string
  description = "The Azure subscription ID"
}

variable "location" {
  type        = string
  description = "The Azure region to deploy to"
}

variable "app_service_plan_name" {
  type        = string
  description = "Name of the App Service Plan"
}

variable "web_app_name" {
  type        = string
  description = "Name of the Web App. Must be globally unique."
}

variable "sql_server_name" {
  type        = string
  description = "Name of the SQL Server. Must be globally unique."
}

variable "sql_database_name" {
  type        = string
  description = "Name of the SQL Database"
}

variable "sql_admin_username" {
  type        = string
  description = "Administrator username for the SQL Server"
}

variable "sql_admin_password" {
  type        = string
  description = "Administrator password for the SQL Server"
  sensitive   = true
}

variable "repo_URL" {
  type        = string
  description = "The URL of the GitHub repository"
  sensitive   = false
}