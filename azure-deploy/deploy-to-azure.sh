#!/bin/bash

# UKNF Platform - Full Azure Deployment Script
# This script deploys the entire solution to Azure using Docker containers

set -e

# Get the script directory and repository root
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPO_ROOT="$( cd "$SCRIPT_DIR/.." && pwd )"

# Change to repository root
cd "$REPO_ROOT"

# Configuration
RESOURCE_GROUP="HackYeah2025"
ACR_NAME="hackyeah2025chlebkiacr"
BACKEND_APP_NAME="hackyeah2025chlebkiknfbackend"
FRONTEND_APP_NAME="hackyeah2025chlebkiknffrontend"
DB_ADMIN_PASSWORD="yeah2025!"
LOCATION="polandcentral"

echo "=========================================="
echo "🚀 UKNF Platform - Azure Deployment"
echo "=========================================="
echo ""
echo "Configuration:"
echo "  Resource Group: $RESOURCE_GROUP"
echo "  ACR Name: $ACR_NAME"
echo "  Backend App: $BACKEND_APP_NAME"
echo "  Frontend App: $FRONTEND_APP_NAME"
echo "  Location: $LOCATION"
echo ""

# Step 1: Ensure resource group exists
echo "📦 Step 1: Ensuring resource group exists..."
if az group show --name $RESOURCE_GROUP &>/dev/null; then
    echo "✅ Resource group '$RESOURCE_GROUP' already exists"
else
    echo "Creating resource group '$RESOURCE_GROUP'..."
    az group create --name $RESOURCE_GROUP --location $LOCATION
    echo "✅ Resource group created"
fi
echo ""

# Step 2: Create ACR (if not exists)
echo "📦 Step 2: Ensuring Azure Container Registry exists..."
if az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP &>/dev/null; then
    echo "✅ ACR '$ACR_NAME' already exists"
else
    echo "Creating ACR '$ACR_NAME'..."
    az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic
    echo "✅ ACR created"
fi

# Enable admin user
echo "Enabling ACR admin user..."
az acr update -n $ACR_NAME --admin-enabled true
echo "✅ ACR admin user enabled"
echo ""

# Step 3: Build and push Docker images
echo "🐳 Step 3: Building and pushing Docker images..."
echo ""

# Login to ACR
echo "Logging in to ACR..."
az acr login --name $ACR_NAME

# Build and push backend
echo "Building backend image..."
docker build -t $ACR_NAME.azurecr.io/uknf-backend:latest ./backend
echo "Pushing backend image..."
docker push $ACR_NAME.azurecr.io/uknf-backend:latest
echo "✅ Backend image pushed"

# Build and push frontend
echo "Building frontend image (production)..."
docker build -f ./frontend/Dockerfile.prod -t $ACR_NAME.azurecr.io/uknf-frontend:latest ./frontend
echo "Pushing frontend image..."
docker push $ACR_NAME.azurecr.io/uknf-frontend:latest
echo "✅ Frontend image pushed"
echo ""

# Verify images
echo "Verifying images in ACR..."
az acr repository list --name $ACR_NAME --output table
echo ""

# Step 4: Deploy infrastructure using Bicep
echo "🏗️  Step 4: Deploying Azure infrastructure..."
az deployment group create \
    --resource-group $RESOURCE_GROUP \
    --template-file azure-deploy/main.bicep \
    --parameters acrName=$ACR_NAME \
                 backendAppName=$BACKEND_APP_NAME \
                 frontendAppName=$FRONTEND_APP_NAME \
                 dbAdminPassword=$DB_ADMIN_PASSWORD

echo "✅ Infrastructure deployed"
echo ""

# Step 5: Restart Web Apps to pull the latest images
echo "🔄 Step 5: Restarting Web Apps..."
az webapp restart --name $BACKEND_APP_NAME --resource-group $RESOURCE_GROUP &
az webapp restart --name $FRONTEND_APP_NAME --resource-group $RESOURCE_GROUP &
wait
echo "✅ Web Apps restarted"
echo ""

# Step 6: Configure PostgreSQL firewall rules
echo "🔐 Step 6: Configuring PostgreSQL firewall rules..."
PG_SERVER_NAME="${BACKEND_APP_NAME}-pg"

# Allow Azure services
az postgres flexible-server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --name $PG_SERVER_NAME \
    --rule-name AllowAllAzureIps \
    --start-ip-address 0.0.0.0 \
    --end-ip-address 0.0.0.0 || echo "Firewall rule may already exist"

echo "✅ PostgreSQL firewall configured"
echo ""

# Step 7: Database migrations
echo "📊 Step 7: Database migrations..."
echo "⚠️  Note: You may need to run migrations manually from the backend app or via SSH"
echo "   Command: az webapp ssh --name $BACKEND_APP_NAME --resource-group $RESOURCE_GROUP"
echo ""

# Final summary
echo "=========================================="
echo "🎉 Deployment Complete!"
echo "=========================================="
echo ""
echo "📍 Resource URLs:"
echo "   Backend API:     https://$BACKEND_APP_NAME.azurewebsites.net"
echo "   Backend Swagger: https://$BACKEND_APP_NAME.azurewebsites.net/swagger"
echo "   Frontend:        https://$FRONTEND_APP_NAME.azurewebsites.net"
echo "   PostgreSQL:      $PG_SERVER_NAME.postgres.database.azure.com"
echo ""
echo "📊 Check deployment status:"
echo "   Backend logs:  az webapp log tail --name $BACKEND_APP_NAME --resource-group $RESOURCE_GROUP"
echo "   Frontend logs: az webapp log tail --name $FRONTEND_APP_NAME --resource-group $RESOURCE_GROUP"
echo ""
echo "🔧 Manage resources:"
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
echo "   Portal: https://portal.azure.com/#@/resource/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP"
echo ""
echo "=========================================="
