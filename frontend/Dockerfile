# Frontend Development Dockerfile with Hot Reload
FROM node:20-alpine

WORKDIR /app

# Install Angular CLI globally
RUN npm install -g @angular/cli@20

# Copy package files
COPY uknf-project/package*.json ./uknf-project/

# Set working directory to project
WORKDIR /app/uknf-project

# Install dependencies
RUN npm install

# Expose port for dev server
EXPOSE 4200

# Start Angular dev server with hot reload
# Using --host 0.0.0.0 to allow external connections from Docker
# Using --poll to ensure file changes are detected in Docker
CMD ["ng", "serve", "--host", "0.0.0.0", "--poll", "2000"]
