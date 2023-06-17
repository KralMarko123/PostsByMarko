#Dockerfile

FROM node:16-alpine AS base 
ENV NODE_ENV development

WORKDIR /src

COPY "PostsByMarko.Client/package.json" .
COPY "PostsByMarko.Client/package-lock.json" .
COPY "PostsByMarko.Client/" .

EXPOSE 3000

RUN npm ci
CMD [ "npm", "start" ]