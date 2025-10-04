#!/bin/bash
# filepath: setup-arch.sh

set -e  # Exit on error

echo "=== HackYeah 2025 - Arch Linux Setup ==="

# Update system
echo "Updating system..."
sudo pacman -Syu --noconfirm

# Install Docker
echo "Installing Docker..."
sudo pacman -S --noconfirm docker docker-compose

# Start and enable Docker
sudo systemctl start docker
sudo systemctl enable docker

# Add current user to docker group
sudo usermod -aG docker $USER

# Install .NET 9 SDK
echo "Installing .NET 9 SDK..."
sudo pacman -S --noconfirm dotnet-sdk

# Install Node.js (for Angular)
echo "Installing Node.js 20..."
sudo pacman -S --noconfirm nodejs npm

# Install Angular CLI
echo "Installing Angular CLI..."
sudo npm install -g @angular/cli@20

# Install Git
echo "Installing Git..."
sudo pacman -S --noconfirm git

# Verify installations
echo ""
echo "=== Installation Verification ==="
docker --version
docker-compose --version
dotnet --version
node --version
npm --version
ng version

echo ""
echo "✅ Setup complete!"
echo "⚠️  IMPORTANT: Log out and log back in for Docker group changes to take effect"
echo ""
echo "Next steps:"
echo "1. Restart your session (logout/login)"
echo "2. Run: docker run hello-world  # Test Docker"
echo "3. Run: ./create-project.sh     # Create project structure"
