#!/bin/bash

# TaskMaster Build Script
# This script builds the Docker containers for TaskMaster

set -e

echo "🚀 Starting TaskMaster build process..."

# Check if .env file exists
if [ ! -f .env ]; then
    echo "⚠️  .env file not found. Creating from .env.example..."
    cp .env.example .env
    echo "✅ Created .env file. Please update it with your configuration."
fi

# Build Docker images
echo "🔨 Building Docker images..."
docker-compose build --no-cache

echo "✅ Build completed successfully!"
echo ""
echo "To start the application, run:"
echo "  ./start.sh"