#!/bin/sh

envsubst '${API_HOSTNAME}' < /etc/nginx/nginx.template.conf > /etc/nginx/conf.d/default.conf

exec nginx -g "daemon off;"