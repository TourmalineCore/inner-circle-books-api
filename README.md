# to-dos-api
Template of REST api service

## What we have in this template?
1. Swagger UI - to see list of endpoints
2. Connection to database with basic entity
3. CRUD operations
4. Structurized logging
5. Exceptions handling
6. Unit testing all application layers
7. E2E karate testing
8. Authorization
9. Tenants for separating data

## How to install template to your dotnet?
Run `dotnet new --install .` command in your terminal.
If you got an error, use `dotnet new --install . --force` command

## How to create project based on this template?
1. Create folder for your project
2. Run `dotnet new to-dos-api -n <replace-with-your-project-name>` command in this folder from your terminal
## How to run project based on this template?
1. Run `docker-compose up -d` command from folder with your project to run database
2. You can access this service by following this link: localhost:35000 or run it in IDE and follow localhost:36000 link
> **_NOTE:_**  You can go to /swagger/index.html page to see available endpoints and test it.

## Running Karate-tests manually
To run Karate tests:
1. Download [karate.jar](https://github.com/karatelabs/karate/releases/download/v1.4.1/karate-1.4.1.jar) file and put it into Tests/E2E folder, also rename it to `karate.jar`
2. Open terminal in this folder
3. Run `java -jar karate.jar ToDoControllerEnd-to-EndTesting.feature`

## Ports
- localhost:36000 - IDE
- localhost:35000 - Docker
- 6432 - postgres port

## Configurations

- MockForPullRequest - used in PR pipeline to run the service in isolation (no external deps) and run its Karate tests against it
- MockForDevelopment - used locally when you run the service in Visual Studio e.g. in Debug and don't want to spin up any external deps
- LocalEnvForDevelopment - used locally when you run the service in Visual Studio and you want to connect to its external deps from Local Env (ToDo not there yet)
- ProdForDevelopment - used locally when you run the service in Visual Studio and want to connect to its external deps from Prod specially dedicated Local Development Tenant (ToDo, need to complete tenants, secrets need to be available in the developer PC env vars)
- ProdForDeployment - used when we run the service in Prod, it shouldn't contain any secrets, it should be a Release build, using real Prod external deps