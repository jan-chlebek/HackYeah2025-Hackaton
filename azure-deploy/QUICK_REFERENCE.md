# Azure Deployment - Quick Reference

## 🚀 One-Command Deployment

### Bash (Linux/macOS/WSL)
```bash
chmod +x azure-deploy/deploy-to-azure.sh && ./azure-deploy/deploy-to-azure.sh
```

### PowerShell (Windows)
```powershell
.\azure-deploy\deploy-to-azure.ps1
```

---

## 📋 What Gets Deployed

| Resource | Name | Description |
|----------|------|-------------|
| Resource Group | HackYeah2025 | Container for all resources |
| Container Registry | hackyeah2025chlebkiacr | Stores Docker images |
| App Service Plan (Backend) | hackyeah2025chlebkiknfbackend-plan | Hosts backend app |
| App Service Plan (Frontend) | hackyeah2025chlebkiknffrontend-plan | Hosts frontend app |
| Web App (Backend) | hackyeah2025chlebkiknfbackend | ASP.NET Core API |
| Web App (Frontend) | hackyeah2025chlebkiknffrontend | Angular SPA |
| PostgreSQL Server | hackyeah2025chlebkiknfbackend-pg | Database |

---

## 🌐 Access URLs

After deployment:
- **Backend API**: https://hackyeah2025chlebkiknfbackend.azurewebsites.net
- **Swagger**: https://hackyeah2025chlebkiknfbackend.azurewebsites.net/swagger
- **Frontend**: https://hackyeah2025chlebkiknffrontend.azurewebsites.net

---

## 🔧 Common Commands

### View Logs
```bash
az webapp log tail --name hackyeah2025chlebkiknfbackend --resource-group HackYeah2025
```

### Restart App
```bash
az webapp restart --name hackyeah2025chlebkiknfbackend --resource-group HackYeah2025
```

### SSH into Container
```bash
az webapp ssh --name hackyeah2025chlebkiknfbackend --resource-group HackYeah2025
```

### Redeploy (after code changes)
```bash
# Rebuild and push images
docker build -t hackyeah2025chlebkiacr.azurecr.io/uknf-backend:latest ./backend
docker push hackyeah2025chlebkiacr.azurecr.io/uknf-backend:latest

# Restart to pull new image
az webapp restart --name hackyeah2025chlebkiknfbackend --resource-group HackYeah2025
```

---

## 🗑️ Clean Up

Delete all resources:
```bash
az group delete --name HackYeah2025 --yes --no-wait
```

---

## 💰 Cost Estimate

Based on Basic SKUs:
- App Service Plan (B1): ~$13/month each (x2 = $26/month)
- Azure Container Registry (Basic): ~$5/month
- PostgreSQL Flexible Server (B1ms): ~$12/month
- **Total**: ~$43/month

> Costs are estimates for Poland Central region. Actual costs may vary.

---

## 🔐 Security Notes

- Database password is set in deployment script (change before production!)
- ACR admin user is enabled (use managed identity for production)
- PostgreSQL allows all Azure IPs (configure specific IPs for production)
- HTTPS is enforced on all Web Apps
- Connection strings and secrets are stored in App Settings
