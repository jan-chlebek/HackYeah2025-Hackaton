# Azure Web App for Containers Deployment Guide

This directory contains assets and instructions to deploy the UKNF Communication Platform to Azure using Web App for Containers.

## Overview

You will:
1. Build and push Docker images for backend and frontend to Azure Container Registry (ACR).
2. Provision Azure resources (ACR, Web Apps, PostgreSQL) using Bicep.
3. Configure Web Apps to use your container images.
4. Set environment variables and secrets.
5. (Optional) Use GitHub Actions for CI/CD.

## Files
- `main.bicep`: Infrastructure-as-Code template for all required Azure resources.
- `deploy-to-azure.sh`: **Full end-to-end deployment script (Bash)**
- `deploy-to-azure.ps1`: **Full end-to-end deployment script (PowerShell)**
- `README.md`: This guide.
- `sample.env`: Example environment variables for local and Azure use.
- `github-actions-sample.yml`: Example GitHub Actions workflow for CI/CD.

---

## Prerequisites
- Azure subscription
- Azure CLI installed and logged in (`az login`)
- Docker installed and running
- Bash (Linux/macOS/WSL) or PowerShell (Windows)
- (Optional) GitHub repository for CI/CD

---

## Quick Start - Automated Deployment

### Option 1: Using Bash Script (Linux/macOS/WSL)

```bash
# Make the script executable
chmod +x azure-deploy/deploy-to-azure.sh

# Run the deployment
./azure-deploy/deploy-to-azure.sh
```

### Option 2: Using PowerShell Script (Windows)

```powershell
# Run the deployment
.\azure-deploy\deploy-to-azure.ps1
```

**Customization:**
Edit the script parameters at the top of the file to customize:
- Resource Group name
- ACR name
- Web App names
- Database password
- Azure region/location

---

## Manual Step-by-Step Deployment

If you prefer manual control or need to troubleshoot, follow these steps:

### 1. Create Resource Group (if needed)
```bash
az group create --name HackYeah2025 --location polandcentral
```

### 2. Create and Configure ACR
```bash
# Create ACR
az acr create --resource-group HackYeah2025 --name hackyeah2025chlebkiacr --sku Basic

# Enable admin user
az acr update -n hackyeah2025chlebkiacr --admin-enabled true

# Login
az acr login --name hackyeah2025chlebkiacr
```

### 3. Build and Push Docker Images
```bash
# Backend
docker build -t hackyeah2025chlebkiacr.azurecr.io/uknf-backend:latest ./backend
docker push hackyeah2025chlebkiacr.azurecr.io/uknf-backend:latest

# Frontend
docker build -t hackyeah2025chlebkiacr.azurecr.io/uknf-frontend:latest ./frontend
docker push hackyeah2025chlebkiacr.azurecr.io/uknf-frontend:latest
```

### 4. Deploy Infrastructure with Bicep
```bash
az deployment group create \
  --resource-group HackYeah2025 \
  --template-file azure-deploy/main.bicep \
  --parameters acrName=hackyeah2025chlebkiacr \
               backendAppName=hackyeah2025chlebkiknfbackend \
               frontendAppName=hackyeah2025chlebkiknffrontend \
               dbAdminPassword="your-secure-password"
```

### 5. Configure Web App Container Images
```bash
# Get ACR credentials
ACR_USERNAME=$(az acr credential show --name hackyeah2025chlebkiacr --query username -o tsv)
ACR_PASSWORD=$(az acr credential show --name hackyeah2025chlebkiacr --query "passwords[0].value" -o tsv)

# Configure backend
az webapp config container set \
  --name hackyeah2025chlebkiknfbackend \
  --resource-group HackYeah2025 \
  --docker-custom-image-name hackyeah2025chlebkiacr.azurecr.io/uknf-backend:latest \
  --docker-registry-server-url https://hackyeah2025chlebkiacr.azurecr.io \
  --docker-registry-server-user $ACR_USERNAME \
  --docker-registry-server-password $ACR_PASSWORD

# Configure frontend
az webapp config container set \
  --name hackyeah2025chlebkiknffrontend \
  --resource-group HackYeah2025 \
  --docker-custom-image-name hackyeah2025chlebkiacr.azurecr.io/uknf-frontend:latest \
  --docker-registry-server-url https://hackyeah2025chlebkiacr.azurecr.io \
  --docker-registry-server-user $ACR_USERNAME \
  --docker-registry-server-password $ACR_PASSWORD
```

### 6. Restart Web Apps
```bash
az webapp restart --name hackyeah2025chlebkiknfbackend --resource-group HackYeah2025
az webapp restart --name hackyeah2025chlebkiknffrontend --resource-group HackYeah2025
```

### 7. Configure PostgreSQL Firewall
```bash
az postgres flexible-server firewall-rule create \
  --resource-group HackYeah2025 \
  --name hackyeah2025chlebkiknfbackend-pg \
  --rule-name AllowAllAzureIps \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

---

## Access Your Deployment

After deployment completes:

- **Backend API**: `https://hackyeah2025chlebkiknfbackend.azurewebsites.net`
- **Backend Swagger**: `https://hackyeah2025chlebkiknfbackend.azurewebsites.net/swagger`
- **Frontend**: `https://hackyeah2025chlebkiknffrontend.azurewebsites.net`
- **PostgreSQL**: `hackyeah2025chlebkiknfbackend-pg.postgres.database.azure.com`

---

## Monitoring and Troubleshooting

### View Logs
```bash
# Backend logs
az webapp log tail --name hackyeah2025chlebkiknfbackend --resource-group HackYeah2025

# Frontend logs
az webapp log tail --name hackyeah2025chlebkiknffrontend --resource-group HackYeah2025
```

### Check Container Status
```bash
az webapp show --name hackyeah2025chlebkiknfbackend --resource-group HackYeah2025 --query state
```

### SSH into Container
```bash
az webapp ssh --name hackyeah2025chlebkiknfbackend --resource-group HackYeah2025
```

---

## CI/CD with GitHub Actions

See `github-actions-sample.yml` for a complete CI/CD workflow. To set up:

1. Copy `github-actions-sample.yml` to `.github/workflows/azure-deploy.yml`
2. Add these secrets to your GitHub repository:
   - `ACR_USERNAME`: From `az acr credential show`
   - `ACR_PASSWORD`: From `az acr credential show`
3. Commit and push to trigger the workflow

---

## Clean Up Resources

To delete all Azure resources:

```bash
az group delete --name HackYeah2025 --yes --no-wait
```

---

## Notes

- **Database Migrations**: Run migrations manually via SSH or deployment slot after first deployment
- **HTTPS**: Enabled by default on all Web Apps
- **Custom Domains**: Configure in Azure Portal under Web App > Custom domains
- **Scaling**: Adjust App Service Plan SKU in `main.bicep` or Azure Portal
