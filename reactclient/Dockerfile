#Dockerfile

FROM node:16-alpine AS base 
ENV NODE_ENV development

WORKDIR /src

COPY "reactclient/package.json" .
COPY "reactclient/package-lock.json" .
COPY "reactclient/" .

RUN npm ci

EXPOSE 3000

CMD [ "npm", "start" ]