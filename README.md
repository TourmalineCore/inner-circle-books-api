# Books API
Service with books in the office for Inner Circle.

## How to run?
### From Docker
Use following command in the terminal:
```bash
docker compose --profile MockForDevelopment up -d
```
### From IDE
Use following command in the terminal to raise the database:
```bash
docker compose --profile db-only up -d
```
After this command, start the application from IDE. 

Go to http://localhost:7000/swagger/index.html to see the list of endpoints and try it using Swagger UI.

## Ports
- localhost:7000 - IDE
- localhost:10012 - Docker
- 10011 - database port

## Configurations
- db-only - profile in docker-compose to raise just database
- MockForPullRequest - used in PR pipeline to run the service in isolation (no external deps) and run its Karate tests against it
- MockForDevelopment - used locally when you run the service in Visual Studio e.g. in Debug and don't want to spin up any external deps
- LocalEnvForDevelopment - used locally when you run the service in Visual Studio and you want to connect to its external deps from Local Env (ToDo not there yet)
- ProdForDevelopment - used locally when you run the service in Visual Studio and want to connect to its external deps from Prod specially dedicated Local Development Tenant (ToDo, need to complete tenants, secrets need to be available in the developer PC env vars)
- ProdForDeployment - used when we run the service in Prod, it shouldn't contain any secrets, it should be a Release build, using real Prod external deps