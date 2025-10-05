# Script to seed Azure PostgreSQL database with initial data
# This runs the seeder against the Azure database

$ErrorActionPreference = "Stop"

Write-Host "Setting up environment for Azure database seeding..." -ForegroundColor Cyan

# Set connection string to Azure PostgreSQL
$env:ConnectionStrings__DefaultConnection = "Server=hackyeah2025chlebkiknfbackend-pg.postgres.database.azure.com;Database=uknf_db;User Id=uknf_user;Password=yeah2025!;Ssl Mode=Require;Trust Server Certificate=true"
$env:ASPNETCORE_ENVIRONMENT = "Development"

Write-Host "Building the seeder application..." -ForegroundColor Cyan
Set-Location -Path ".\backend\UknfCommunicationPlatform.Api"

# Build the project
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}

Write-Host "Running database seeder..." -ForegroundColor Cyan
Write-Host "This will populate the Azure database with initial data..." -ForegroundColor Yellow

# Run the application (it will seed on startup in Development mode)
# We'll let it run briefly then stop it
$process = Start-Process -FilePath "dotnet" -ArgumentList "run --no-build --configuration Release" -PassThru -NoNewWindow

Write-Host "Waiting for seeding to complete (30 seconds)..." -ForegroundColor Cyan
Start-Sleep -Seconds 30

Write-Host "Stopping the application..." -ForegroundColor Cyan
Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue

Write-Host "Database seeding completed!" -ForegroundColor Green
Write-Host "Testing FAQ endpoint..." -ForegroundColor Cyan

# Test the endpoint
Start-Sleep -Seconds 2
$response = Invoke-WebRequest -Uri "https://hackyeah2025chlebkiknfbackend.azurewebsites.net/api/v1/Faq?page=1&pageSize=5" -Method GET
$data = $response.Content | ConvertFrom-Json

Write-Host "FAQ endpoint returned $($data.totalCount) total items" -ForegroundColor Green

Set-Location -Path "../.."
