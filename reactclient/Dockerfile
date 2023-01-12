FROM node:16-alpine AS base 
ENV NODE_ENV development

WORKDIR /src

COPY ["reactclient/package.json", "reactclient/"]
COPY ["reactclient/package-lock.json", "reactclient/"]
COPY . .

WORKDIR "/src/reactclient"
RUN npm ci

EXPOSE 3000

CMD [ "npm", "start" ]