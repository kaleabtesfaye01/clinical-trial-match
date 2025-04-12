FROM node:slim AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build:prod
FROM nginx:1.27.4-alpine-slim
COPY default.conf /etc/nginx/conf.d/
COPY --from=build /app/dist/client/browser /usr/share/nginx/html
EXPOSE 80