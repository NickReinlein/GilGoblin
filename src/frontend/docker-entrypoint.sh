#!/bin/sh
set -e

envsubst '${API_HOSTNAME}' < /etc/nginx/nginx.template.conf > /etc/nginx/conf.d/default.conf

echo "Starting Nginx..."

exec nginx -g "daemon off;"