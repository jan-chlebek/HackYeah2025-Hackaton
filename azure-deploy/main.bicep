// main.bicep - Azure infrastructure for UKNF Platform (Web App for Containers)
// Deploys: Resource Group, ACR, Web Apps (backend/frontend), PostgreSQL

param location string = resourceGroup().location
param acrName string
param backendAppName string
param frontendAppName string
param dbAdminUsername string = 'uknf_user'
@secure()
param dbAdminPassword string
param dbName string = 'uknf_db'

resource acr 'Microsoft.ContainerRegistry/registries@2023-01-01-preview' = {
  name: acrName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

// Use a supported API version for your region (e.g., 2022-12-01 for polandcentral)
resource postgres 'Microsoft.DBforPostgreSQL/flexibleServers@2022-12-01' = {
  name: '${backendAppName}-pg'
  location: location
  properties: {
    administratorLogin: dbAdminUsername
    administratorLoginPassword: dbAdminPassword
    version: '15'
    storage: {
      storageSizeGB: 32
    }
    createMode: 'Default'
    highAvailability: {
      mode: 'Disabled'
    }
    backup: {
      backupRetentionDays: 7
      geoRedundantBackup: 'Disabled'
    }
  }
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
}

// Shared App Service Plan - Premium v3 P2V3
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: '${backendAppName}-plan'
  location: location
  sku: {
    name: 'P2V3'
    tier: 'PremiumV3'
    size: 'P2V3'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource backendApp 'Microsoft.Web/sites@2023-01-01' = {
  name: backendAppName
  location: location
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|${acrName}.azurecr.io/uknf-backend:latest'
      acrUseManagedIdentityCreds: false
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: 'Server=${postgres.name}.postgres.database.azure.com;Database=${dbName};User Id=${dbAdminUsername}@${postgres.name};Password=${dbAdminPassword};Ssl Mode=Require;Trust Server Certificate=true'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: 'https://${acrName}.azurecr.io'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_USERNAME'
          value: acr.listCredentials().username
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_PASSWORD'
          value: acr.listCredentials().passwords[0].value
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
      ]
    }
    httpsOnly: true
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource frontendApp 'Microsoft.Web/sites@2023-01-01' = {
  name: frontendAppName
  location: location
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOCKER|${acrName}.azurecr.io/uknf-frontend:latest'
      acrUseManagedIdentityCreds: false
      appSettings: [
        {
          name: 'WEBSITES_PORT'
          value: '4000'
        }
        {
          name: 'PORT'
          value: '4000'
        }
        {
          name: 'API_URL'
          value: 'https://${backendAppName}.azurewebsites.net/api/v1/'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: 'https://${acrName}.azurecr.io'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_USERNAME'
          value: acr.listCredentials().username
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_PASSWORD'
          value: acr.listCredentials().passwords[0].value
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
      ]
    }
    httpsOnly: true
  }
  identity: {
    type: 'SystemAssigned'
  }
}
