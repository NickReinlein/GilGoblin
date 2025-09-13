#!/bin/sh
set -e

host="$1"
port="$2"
shift
shift

echo "Waiting for DNS resolution of $host..."
until getent hosts "$host"; do
  echo "DNS resolution failed, retrying..."
  sleep 1
done

echo "DNS resolution successful. Waiting for host $host to be available on port $port..."
until nc -z "$host" "$port"; do
  echo "Host $host not available on port $port, retrying..."
  sleep 1
done

echo "Host $host is up, starting service."
exec "$@"