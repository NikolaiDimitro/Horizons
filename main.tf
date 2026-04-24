terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.69.0"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

# Random Integer for unique resource naming
resource "random_integer" "ri" {
  min = 10000
  max = 99999
}

# Resource Group
resource "azurerm_resource_group" "rg" {
  name     = "${var.resource_group_name}-${random_integer.ri.result}"
  location = var.location
}

# App Service Plan
resource "azurerm_service_plan" "asp" {
  name                = "${var.app_service_plan_name}-${random_integer.ri.result}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "F1"
}

# Azure Web App (.NET)
resource "azurerm_linux_web_app" "app" {
  name                = "${var.web_app_name}-${random_integer.ri.result}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_service_plan.asp.location
  service_plan_id     = azurerm_service_plan.asp.id

  site_config {
    always_on = false # Required for F1 tier
    application_stack {
      dotnet_version = "8.0"
    }
  }

  # Connection string linking the Web App to the SQL DB
  connection_string {
    name  = "DefaultConnection"
    type  = "SQLAzure"
    value = "Data Source=tcp:${azurerm_mssql_server.sqlserver.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.database.name};User ID=${azurerm_mssql_server.sqlserver.administrator_login};Password=${azurerm_mssql_server.sqlserver.administrator_login_password};Trusted_Connection=False; MultipleActiveResultSets=True;"
  }
}

# Azure SQL Server
resource "azurerm_mssql_server" "sqlserver" {
  name                         = "${var.sql_server_name}-${random_integer.ri.result}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
}

# Azure SQL Database
resource "azurerm_mssql_database" "database" {
  name                 = "${var.sql_database_name}-${random_integer.ri.result}"
  server_id            = azurerm_mssql_server.sqlserver.id
  sku_name             = "S0"
  storage_account_type = "Local"
  zone_redundant       = false
}

# Allow Azure Services (like the Web App) to access the SQL Database
resource "azurerm_mssql_firewall_rule" "firewall" {
  name             = "AllowAllWindowsAzureIps"
  server_id        = azurerm_mssql_server.sqlserver.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# GitHub Source Control Integration
resource "azurerm_app_service_source_control" "github" {
  app_id                 = azurerm_linux_web_app.app.id
  repo_url               = var.repo_URL
  branch                 = "main"
  use_manual_integration = false
}