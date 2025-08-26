# Stage 1: Build React app
FROM node:20 AS builder
WORKDIR /app
COPY ["ITC.Frontend/package*.json", "."]
RUN npm install
COPY ITC.Frontend/ .
RUN npm run build

# Stage 2: Serve with Nginx
FROM nginx:1.25-alpine
COPY --from=builder /app/dist /usr/share/nginx/html
COPY ["ITC.Frontend/nginx/nginx.conf", "/etc/nginx/conf.d/default.conf"]
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]