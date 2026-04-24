output "webapp_url" {
  value       = "https://${azurerm_linux_web_app.app.default_hostname}"
  description = "The URL of the deployed Web App"
}

output "sql_server_fqdn" {
  value       = azurerm_mssql_server.sqlserver.fully_qualified_domain_name
  description = "The fully qualified domain name of the SQL Server"
}