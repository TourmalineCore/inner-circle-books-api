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

## How to run Karate tests locally?
**Firstly, raise all the necessary services in Docker, following this instruction (not in dev container):**
1. Open the project in VS Code.
2. Enter this command in the terminal, opened in project's folder
```
docker compose --profile MockForPullRequest up -d
```
**Then, open dev container and run the tests following these steps:**
1. In **Visual Studio Code** install the extension "Dev Containers" (extension id: ms-vscode-remote.remote-containers)
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
- LocalEnvForPullRequest - used in PR pipeline to run the service in isolation in Local Env (ToDo not there yet)
- ProdForDeployment - used when we run the service in Prod, it shouldn't contain any secrets, it should be a Release build, using real Prod external deps

## How add new migration

When you have made all the changes and are ready to add new migration, you need to follow these steps

1. You need to run db in docker, using command

```
docker compose --profile db-only up -d
```

### for MacOS

2. Then add new migration, using the following command and dont forget to change `YourNewMigrationName`

```
dotnet ef migrations add YourNewMigrationName --startup-project Api/ --project Application/ -- --environment MockForDevelopment
```

3. After adding your new migration you need to update db, using the following command

```
dotnet ef database update --startup-project Api/ --project Application/ -- --environment MockForDevelopment
```

4. If you want to remove `last` migration, use the following command

```
dotnet ef migrations remove --startup-project Api/ --project Application/ -- --environment MockForDevelopment
```

### for Windows

#### TODO: Most likely it will be so, but it is necessary to check on Windows the correctness of the work and if something is wrong here, then correct the README

2. Then add new migration, using the following command and dont forget to change `YourNewMigrationName`

```
dotnet ef migrations add YourNewMigrationName -- --environment MockForDevelopment
```

3. After adding your new migration you need to update db, using the following command

```
dotnet ef database update -- --environment MockForDevelopment
```

4. If you want to remove `last` migration, use the following command

```
dotnet ef migrations remove -- --environment MockForDevelopment
```

## Database scheme 
```mermaid
erDiagram
    Books ||--o{ BooksCopies : "1-to-many"
    BooksCopies ||--o{ BooksCopiesReadingHistory : "1-to-many"
    Books {
        long id PK "Example: '1'"
        long tenantId "Example: '1'"
        text title "Example: 'Пиши, сокращай 2025: Как создавать сильный текст'" 
        text annotation "Example: 'Книга о создании текста для всех, кто пишет по работе'"
        text authors "Example: '[{'fullName': 'Максим Ильяхов'}, {'fullName': 'Людмила Сарычева'}]'"
        enum language "Example: 'ru'"
        datetime da "Example: '2024-12-25 09:20:25.695197+00'"
        datetime deletedAtUtc "nullable, Example: '2024-12-25 09:20:25.695197+00'"
        text artworkUrl "nullable, Example: 'https://cdn.litres.ru/pub/c/cover/70193008.jpg'"
    }
    BooksCopies {
        long id PK "Example: '1'"
        long bookId FK "Example: '1'"
    }
    BooksCopiesReadingHistory {
        long id PK "Example: '1'"
        long bookCopyId FK "Example: '1'"
        long readerEmployeeId "Example: '1'"
        datetime takenAtUtc "Example: '2024-12-25 09:20:25.695197+00'"
        date sheduledReturnDate "Example: '2024-12-25'"
        datetime actualReturnedAtUtc "Nullable" "Example: '2024-12-25 09:20:25.695197+00'"
    }
```
