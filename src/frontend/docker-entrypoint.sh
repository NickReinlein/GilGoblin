#!/bin/sh
set -e

envsubst '${API_HOSTNAME}' < /etc/nginx/nginx.template.conf > /etc/nginx/conf.d/default.conf

echo "Waiting for DNS record to resolve..."
while ! ping -c 1 "$API_HOSTNAME" > /dev/null 2>&1; do
echo "Still waiting for $API_HOSTNAME..."
sleep 1
done

echo "DNS resolved. Starting Nginx..."

exec nginx -g "daemon off;"