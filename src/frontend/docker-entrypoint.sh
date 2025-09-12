#!/bin/sh

envsubst '${API_HOSTNAME}' < /etc/nginx/nginx.template.conf > /etc/nginx/conf.d/default.conf

/wait-for-nginx.sh "$API_HOSTNAME" 55448 -- nginx -g "daemon off;"