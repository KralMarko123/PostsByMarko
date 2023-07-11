#Dockerfile

FROM node:16-alpine AS build-env 
ARG CLIENT_PATH="src/PostsByMarko.Client"

ENV NODE_ENV development

WORKDIR /app

COPY ${CLIENT_PATH}/package.json .
COPY ${CLIENT_PATH}/package-lock.json .
COPY ${CLIENT_PATH}/ .

EXPOSE 3000

RUN npm ci
CMD [ "npm", "start" ]