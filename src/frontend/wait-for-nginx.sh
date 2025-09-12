#!/bin/sh
# Wait for the host to be available
host="$1"
port="$2"
shift 2

while ! nc -z "$host" "$port"; do
  echo "Waiting for host $host on port $port..."
  sleep 1
done

echo "Host $host is up, starting service."
exec "$@"