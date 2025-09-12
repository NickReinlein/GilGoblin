#!/bin/sh

envsubst '${API_HOSTNAME}' < /etc/nginx/nginx.template.conf > /etc/nginx/conf.d/default.conf

echo "Waiting for DNS record to propagate..."
sleep 10 # Wait to give DNS a chance to register

/wait-for-nginx.sh "$API_HOSTNAME" 55448 -- nginx -g "daemon off;"