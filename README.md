# Books API

Service with books in the office for Inner Circle.

## Run in Visual Studio

First run this script to run a db and mocked external deps:

```bash
docker compose --profile MockForDevelopment up --build
```

## Karate Tests

### Run Karate against Api, Db, and MockServer in Docker Compose

Run Api, Db, and MockServer executing the following command (don't close the terminal unless you want to stop the containers)

```bash
docker compose --profile MockForTests up --build
```

Then execute following command inside of the dev-container

```bash
java -jar /karate.jar .
```

### Running Karate Tests, Api, Db, and MockServer in Docker Compose

Run the docker compose with MockForPullRequest profile executing the following command (don't close the terminal unless you want to stop the containers)

```bash
docker compose --profile MockForPullRequest up --build
```

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