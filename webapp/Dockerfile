FROM node:18 as build
WORKDIR /app

RUN npm install -g @angular/cli

COPY ./package.json .
#RUN npm install
RUN npm install --legacy-peer-deps
COPY . .

RUN ng config -g cli.warnings.versionMismatch false
RUN ng build

EXPOSE 80
FROM nginx as runtime
COPY --from=build /app/dist/webapp /usr/share/nginx/html
