# Docker registry .NET Core 6 Backend API's and Angular Web App to control and escape Docker containers

## Services

Git Actions are defined for each of `web_api`, `docker_api` and `web_app`. On each new commit Docker containers are built automatically and pushed to private Docker Hub repo. Each action has a filter which triggers a new build only when there are changes inside the underlying folder (e.g web_api folder for `web_api` service). Builds run in parallel and finish in 2 minutes tops.

Docker-compose is used to build and orchestrate all the services. Each of them could be started separately and locally as well.

`web_api` service contains C# backend API with DB (Postgres for production or Sqlite for dev) and Entity Framework.

`docker_api` service contains C# backend API to control Docker containers and images. Underlying Docker container is escaped and is able to comunicate with Host's Docker API. This API does not comunicate directly to DB. That might not be smart as the container is escaped. It can comunicate with `web_api` via `web_app`.

`web_app` service contains Angular Web App which consumes both API's and controls Docker containers and images. App is able to download containers even from private Docker Hub repo.

`postgres_db` service contains PostgresDB.

`pgadmin` service contains pgadmin4 to interract with PostgresDB. PostgresDB is automatically provisioned as a server inside `pgadmin`.

`redis` service contains in memory Redis DB. It was meant to be used as cache, but it could also help solve complex problems.


## Build

Be awere that complete build could last more than 10min hammering the CPU and use more than 15GB of storage!

To build containers navigate to the root folder containing `docker-compose.yml` file and execute: 
`docker-compose up --build --detach`

Inside Docker `docker_api` runs on http://localhost:8000

Inside Docker `web_api` runs on http://localhost:8001

Inside Docker `web_app` runs on http://localhost:4200

Tested on: `macOS Monterey 12.6` and latest `Archlinux`

## Documentation

Both API's have Swagger enabled.

![alt text](https://github.com/APback2331/docker_reg/blob/main/scr.png?raw=true)
