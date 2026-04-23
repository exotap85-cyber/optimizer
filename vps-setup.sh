set -e
export DEBIAN_FRONTEND=noninteractive
apt-get update -y
apt-get install -y curl unzip mysql-server
if ! command -v node >/dev/null 2>&1; then
  curl -fsSL https://deb.nodesource.com/setup_18.x | bash -
  apt-get install -y nodejs
fi
systemctl enable --now mysql
mkdir -p /var/www/gamer-optimizer-license-server
cd /var/www/gamer-optimizer-license-server
unzip -o /root/backend-deploy.zip
if [ ! -f .env ]; then
  cp .env.example .env
fi
DB_NAME=gamer_optimizer
DB_USER=gamer_optimizer
DB_PASS=$(openssl rand -hex 16)
mysql -uroot <<SQL
CREATE DATABASE IF NOT EXISTS ${DB_NAME};
CREATE USER IF NOT EXISTS '${DB_USER}'@'localhost' IDENTIFIED BY '${DB_PASS}';
GRANT ALL PRIVILEGES ON ${DB_NAME}.* TO '${DB_USER}'@'localhost';
FLUSH PRIVILEGES;
SQL
mysql -uroot ${DB_NAME} < database.sql
sed -i "s/^DB_HOST=.*/DB_HOST=localhost/" .env
sed -i "s/^DB_USER=.*/DB_USER=${DB_USER}/" .env
sed -i "s/^DB_PASSWORD=.*/DB_PASSWORD=${DB_PASS}/" .env
sed -i "s/^DB_NAME=.*/DB_NAME=${DB_NAME}/" .env
sed -i "s/^PORT=.*/PORT=8080/" .env
npm install
cat >/etc/systemd/system/gamer-optimizer-license.service <<'SERVICE'
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
SERVICE
systemctl daemon-reload
systemctl enable gamer-optimizer-license
systemctl restart gamer-optimizer-license
systemctl status gamer-optimizer-license --no-pager
printf '\nDB_USER=%s\nDB_PASS=%s\n' "$DB_USER" "$DB_PASS"