FROM node:24-alpine AS dependencies
ARG CLIENT_PATH="src/PostsByMarko.Client"
WORKDIR /app
COPY ${CLIENT_PATH}/package.json ${CLIENT_PATH}/package-lock.json ./
RUN npm ci

FROM dependencies AS development
ARG CLIENT_PATH="src/PostsByMarko.Client"
COPY ${CLIENT_PATH}/ ./
EXPOSE 3000
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "3000"]

FROM dependencies AS build
ARG CLIENT_PATH="src/PostsByMarko.Client"
ARG VITE_API_BASE_URL
ENV VITE_API_BASE_URL=${VITE_API_BASE_URL}
COPY ${CLIENT_PATH}/ ./
RUN npm run build

FROM nginx:1.29-alpine AS production
COPY src/PostsByMarko.Client/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
