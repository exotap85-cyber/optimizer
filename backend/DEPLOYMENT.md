# Gamer Optimizer - License Server Deployment Guide

## VPS Setup Instructions

This guide will help you set up the license validation server on your VPS at `69.10.60.15`.

### Prerequisites

- Node.js 14+ installed on VPS
- MySQL 5.7+ installed on VPS
- SSH access to VPS
- Domain or IP access

### Step 1: Connect to VPS

```bash
ssh root@69.10.60.15
```

Password: `exo1tap`

### Step 2: Install Node.js (if not already installed)

```bash
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs
```

### Step 3: Create Application Directory

```bash
mkdir -p /var/www/gamer-optimizer-license-server
cd /var/www/gamer-optimizer-license-server
```

### Step 4: Copy Application Files

Upload the following files to your VPS:
- `license-server.js`
- `package.json`
- `.env` (customize with your database credentials)

### Step 5: Install Dependencies

```bash
npm install
```

### Step 6: Create MySQL Database

```bash
mysql -u root -p < database.sql
```

Or manually:
```bash
mysql -u root -p
CREATE DATABASE gamer_optimizer;
USE gamer_optimizer;
```

Then import the schema from `database.sql`.

### Step 7: Configure Environment Variables

```bash
cp .env.example .env
nano .env
```

Update the following:
- `DB_HOST`: Your MySQL host (localhost if on same server)
- `DB_USER`: MySQL username
- `DB_PASSWORD`: MySQL password
- `DB_NAME`: gamer_optimizer
- `PORT`: 8080

### Step 8: Start the Server

```bash
# Development (with auto-restart)
npm install -g nodemon
npm run dev

# Production (background)
nohup npm start > /var/log/gamer-optimizer.log 2>&1 &
```

### Step 9: Setup Systemd Service (Recommended for Production)

Create `/etc/systemd/system/gamer-optimizer-license.service`:

```ini
[Unit]
Description=Gamer Optimizer License Server
After=network.target

[Service]
Type=simple
User=www-data
WorkingDirectory=/var/www/gamer-optimizer-license-server
ExecStart=/usr/bin/node /var/www/gamer-optimizer-license-server/license-server.js
Restart=on-failure
RestartSec=10

[Install]
WantedBy=multi-user.target
```

Enable and start:
```bash
systemctl enable gamer-optimizer-license
systemctl start gamer-optimizer-license
systemctl status gamer-optimizer-license
```

### Step 10: Setup Nginx Reverse Proxy (Optional)

```bash
sudo apt-get install -y nginx
```

Create `/etc/nginx/sites-available/license-server`:

```nginx
server {
    listen 8080;
    server_name 69.10.60.15;

    location /api/ {
        proxy_pass http://localhost:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

Enable:
```bash
sudo ln -s /etc/nginx/sites-available/license-server /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

### Step 11: Verify Installation

Test the health endpoint:
```bash
curl http://69.10.60.15:8080/api/health
```

Should respond with:
```json
{"status":"ok","message":"License server is running"}
```

### Step 12: Test License Validation

```bash
curl -X POST http://69.10.60.15:8080/api/validate-key \
  -H "Content-Type: application/json" \
  -d '{"license_key":"GAMOPT-2026-TEST-001","hwid":"TEST_HWID"}'
```

## Managing Licenses

### Adding a New License

```bash
mysql -u root -p gamer_optimizer
INSERT INTO licenses (license_key, user_email, status, expiry_date)
VALUES ('GAMOPT-2026-NEWKEY-001', 'user@example.com', 'active', DATE_ADD(NOW(), INTERVAL 1 YEAR));
```

### Checking License Status

```bash
SELECT * FROM licenses WHERE license_key = 'GAMOPT-2026-TEST-001';
```

### Viewing Validation Logs

```bash
SELECT * FROM license_logs ORDER BY created_at DESC LIMIT 20;
```

## Troubleshooting

### Cannot connect to database
- Check MySQL is running: `systemctl status mysql`
- Verify credentials in `.env`
- Check MySQL user has proper permissions

### Port 8080 already in use
```bash
lsof -i :8080
kill -9 <PID>
```

### Server not responding
- Check logs: `tail -f /var/log/gamer-optimizer.log`
- Verify firewall: `sudo ufw allow 8080`
- Check Node.js process: `ps aux | grep node`

## API Endpoints

### POST /api/validate-key
Validate a license key

**Request:**
```json
{
  "license_key": "GAMOPT-2026-TEST-001",
  "hwid": "YOUR_HARDWARE_ID"
}
```

**Response (Valid):**
```json
{
  "status": "valid",
  "message": "License is valid",
  "expiry_date": "2026-12-31"
}
```

**Response (Expired):**
```json
{
  "status": "expired",
  "message": "License has expired",
  "expiry_date": "2025-12-31"
}
```

**Response (Invalid):**
```json
{
  "status": "invalid",
  "message": "License key not found"
}
```

### GET /api/health
Server health check

**Response:**
```json
{
  "status": "ok",
  "message": "License server is running"
}
```

## Security Recommendations

1. **Use HTTPS**: Setup SSL certificate using Let's Encrypt
2. **Rate Limiting**: Add express-rate-limit to prevent abuse
3. **Input Validation**: Validate and sanitize all inputs
4. **Database Security**: Use strong passwords, limit user permissions
5. **Logging**: Monitor license_logs table for suspicious activity
6. **Backup**: Regular MySQL backups
7. **Firewall**: Only allow necessary ports (80, 443, 8080)

## License Key Format

Generate license keys with a consistent format:
- `GAMOPT-YYYY-TYPE-###`
- Example: `GAMOPT-2026-PROMO-001`
- Can be customized in your database

## Support

For issues or updates, contact the development team.
