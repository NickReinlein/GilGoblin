#!/bin/sh

# Use envsubst to replace the API_HOSTNAME placeholder
envsubst '${API_HOSTNAME}' < /etc/nginx/nginx.template.conf > /etc/nginx/conf.d/default.conf

# Call the wait-for-nginx script
/wait-for-nginx.sh "$API_HOSTNAME" 55448 -- nginx -g "daemon off;"