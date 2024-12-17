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

## Karate tests

To run tests, you need to open the project in VS Code and Visual Studio.
Enter this command in Visual Studio
```
docker-compose --profile MockForPullRequest up -d
```

Go to VS Code
1. Install the extension "Dev Containers" (extension id: ms-vscode-remote.remote-containers)
2. Click on the blue button in the lower left corner of your screen
3. Click "Rebuild Container" or something like this - the project will start in dev container
4. Enter this command to run the tests
```
java -jar /karate.jar .
```

### How to edit test auth token in Karate test?
1. Copy token from initializer.json (accessToken value) file and insert it into some Base64 Decoder;
2. Change values;
3. Encode to Base64;
4. Insert to initializer.json file, instead of old accessToken .

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