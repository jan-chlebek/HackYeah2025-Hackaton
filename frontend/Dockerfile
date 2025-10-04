# Development Dockerfile for Angular with hot reload
FROM node:20-alpine

WORKDIR /app/uknf-project

# Copy package files from uknf-project
COPY uknf-project/package*.json ./

# Install all dependencies (including dev dependencies for Angular CLI)
RUN npm install

# Copy source code from uknf-project
COPY uknf-project/ .

# Expose Angular dev server port
EXPOSE 4200

# Start Angular development server with host binding and file watching
CMD ["npx", "ng", "serve", "--host", "0.0.0.0", "--poll", "2000"]
