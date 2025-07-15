# Library Management System

This is a simple Library Management System implemented in C# using Entity Framework Core with SQLite as the database provider.

## Features

- Add, retrieve, and delete books.
- User registration and authentication.
- JWT-based token authentication.
- Pagination support for listing books.

## Technology Stack

- C# (.NET 6 or later)
- Entity Framework Core
- SQLite database
- xUnit for unit testing

## Database

This project uses **SQLite** as the database provider. The SQLite database file is created automatically the first time you run the application. No manual setup is required.

You can use **DB Browser for SQLite** or any other SQLite client to open and inspect the database file to confirm the data and schema.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQLite client (optional, for inspecting the database)

### Running the Application

1. Clone the repository:

   ```bash
   git clone https://github.com/Chuks-Onuh/LibraryManagementSystem.git
   ```
1. Build and run the application:

    ```bash
    dotnet build
    ```

2. The SQLite database file (`library.db`) will be created automatically in the project directory on first run.

3. Use **DB Browser for SQLite** or any SQLite client to open and explore the database.
