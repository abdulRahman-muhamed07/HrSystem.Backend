#!/bin/bash
# ============================================================
# HR System - Production Deployment Script
# Tested on: Ubuntu 22.04 / 24.04
# ============================================================
set -e

APP_DIR="/var/www/hr-system"
SERVICE_NAME="hr-system"

BLUE='\033[0;34m'
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

log()  { echo -e "${BLUE}[DEPLOY]${NC} $1"; }
ok()   { echo -e "${GREEN}[OK]${NC} $1"; }
fail() { echo -e "${RED}[FAIL]${NC} $1"; exit 1; }

# 1. Install .NET 8 if not present
log "Checking .NET 8 SDK..."
if ! command -v dotnet &> /dev/null; then
    log "Installing .NET 8 SDK..."
    sudo apt-get update -qq
    sudo apt-get install -y -qq wget apt-transport-https
    wget -q https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
    chmod +x /tmp/dotnet-install.sh
    /tmp/dotnet-install.sh --channel 8.0 --install-dir /usr/share/dotnet
    sudo ln -sf /usr/share/dotnet/dotnet /usr/bin/dotnet
    rm /tmp/dotnet-install.sh
fi
ok ".NET SDK $(dotnet --version)"

# 2. Create app directory
log "Setting up application directory..."
sudo mkdir -p "$APP_DIR"
sudo chown $USER:$USER "$APP_DIR"

# 3. Copy project files
log "Copying project files..."
cp -r . "$APP_DIR/"

# 4. Build
log "Building the project..."
cd "$APP_DIR"
dotnet restore --verbosity quiet
dotnet publish -c Release -o "$APP_DIR/publish" --verbosity quiet
ok "Build successful"

# 5. Set permissions
log "Setting permissions..."
sudo chown -R www-data:www-data "$APP_DIR/publish"
sudo chmod -R 755 "$APP_DIR/publish"

# 6. Install systemd service
log "Installing systemd service..."
sudo cp deploy/$SERVICE_NAME.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable $SERVICE_NAME
sudo systemctl restart $SERVICE_NAME

# 7. Wait and check status
sleep 3
if sudo systemctl is-active --quiet $SERVICE_NAME; then
    ok "Service is running!"
else
    fail "Service failed to start. Check: sudo journalctl -u $SERVICE_NAME -n 50"
fi

# 8. Test the API
curl -s http://localhost:5000/api/WeatherForecast > /dev/null 2>&1 && ok "API health check passed" || fail "API not responding"

echo ""
echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}  DEPLOYMENT COMPLETE!${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""
echo "  API URL:     http://$(hostname -I | awk '{print $1}'):5000"
echo "  Health:      http://localhost:5000/api/WeatherForecast"
echo "  Frontend:    http://$(hostname -I | awk '{print $1}'):5000"
echo ""
echo "  Test Accounts:"
echo "    Admin:     admin@company.com / Admin123!"
echo "    Employee:  emp@company.com / Emp123!"
echo ""
echo "  Useful commands:"
echo "    sudo systemctl status $SERVICE_NAME"
echo "    sudo journalctl -u $SERVICE_NAME -f"
echo "    sudo systemctl restart $SERVICE_NAME"
echo ""
