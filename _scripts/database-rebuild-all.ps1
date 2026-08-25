# Kör: powershell -ExecutionPolicy Bypass -File database-rebuild-all.ps1

$ErrorActionPreference = "Continue"  # don't let stderr output from dotnet trigger false failures

Set-Location (Join-Path $PSScriptRoot "..")

Write-Host "======================================"
Write-Host "        Resetting SaaS Database"
Write-Host "======================================"

$Project = "DbContext/DbContext.csproj"
$StartupProject = "AppWebApi/AppWebApi.csproj"

Write-Host ""
Write-Host "Working directory:"
Get-Location

Write-Host ""
Write-Host "1. Dropping database..."

dotnet ef database drop --project $Project --startup-project $StartupProject --force
if ($LASTEXITCODE -ne 0) { Write-Error "Database drop failed."; exit 1 }

Write-Host ""
Write-Host "2. Removing migrations..."

do {
    dotnet ef migrations remove --project $Project --startup-project $StartupProject --force
    $success = ($LASTEXITCODE -eq 0)
    if ($success) { Write-Host "Migration removed." }
} while ($success)

Write-Host ""
Write-Host "3. Creating InitialCreate migration..."

dotnet ef migrations add InitialCreate --project $Project --startup-project $StartupProject
if ($LASTEXITCODE -ne 0) { Write-Error "Migration creation failed."; exit 1 }

Write-Host ""
Write-Host "4. Updating database..."

dotnet ef database update --project $Project --startup-project $StartupProject
if ($LASTEXITCODE -ne 0) { Write-Error "Database update failed."; exit 1 }

Write-Host ""
Write-Host "======================================"
Write-Host "        Database reset complete!"
Write-Host "======================================"