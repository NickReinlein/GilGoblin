#!/bin/sh

if [ -n "$DATABASE_URL" ]; then
  export POSTGRES_DB=$(echo $DATABASE_URL | sed -r 's/.*Database=([^;]+).*/\1/')
  export POSTGRES_USER=$(echo $DATABASE_URL | sed -r 's/.*Username=([^;]+).*/\1/')
  export POSTGRES_PASSWORD=$(echo $DATABASE_URL | sed -r 's/.*Password=([^;]+).*/\1/')
  export POSTGRES_HOST=$(echo $DATABASE_URL | sed -r 's/.*Host=([^;]+).*/\1/')
  export POSTGRES_PORT=$(echo $DATABASE_URL | sed -r 's/.*Port=([^;]+).*/\1/')
fi

# Call the original entrypoint
exec docker-entrypoint.sh "$@"
