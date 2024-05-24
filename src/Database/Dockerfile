FROM postgres:alpine

# Copy the custom entrypoint script into the container
COPY docker-entrypoint.sh /usr/local/bin/

# Ensure the entrypoint script is executable
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

# Use the custom entrypoint script
ENTRYPOINT ["docker-entrypoint.sh"]

EXPOSE 5432
