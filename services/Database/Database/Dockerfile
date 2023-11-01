FROM postgres:alpine

ENV POSTGRES_DB=gilgoblin
ENV POSTGRES_USER=gilgoblin
ENV POSTGRES_PASSWORD=gilgoblin
ENV PGUSER=gilgoblin
ENV PGDATA=/var/lib/postgresql/data

COPY ./db /var/lib/postgresql/data
COPY ./resources /docker-entrypoint-initdb.d/

EXPOSE 5432