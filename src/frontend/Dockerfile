FROM node:18-alpine AS builder

WORKDIR /app

COPY package*.json .

RUN npm install

COPY . .

EXPOSE 3000

RUN npm run build

FROM nginx:1.21.0-alpine as production
ENV NODE_ENV production

COPY --from=builder /app/build /usr/share/nginx/html

COPY nginx.template.conf /etc/nginx/
COPY docker-entrypoint.sh /
COPY wait-for-nginx.sh /

RUN chmod +x /docker-entrypoint.sh /wait-for-nginx.sh

EXPOSE 3000

CMD ["/docker-entrypoint.sh"]