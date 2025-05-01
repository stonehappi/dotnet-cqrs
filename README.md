# 📦 DotnetCqrs (Manual CQRS in ASP.NET Core)

A beginner-friendly .NET 9 project demonstrating the **Command Query Responsibility Segregation (CQRS)** pattern **without using MediatR**, written in a clean and simple structure for learning.

---

## 🧠 What is CQRS?

CQRS stands for **Command Query Responsibility Segregation**. It's a software design pattern that separates:
- ✅ **Commands** (write operations – like Create, Update, Delete)
- ✅ **Queries** (read operations – like Get, List)

In this project, these responsibilities are implemented manually using clear folder separation and handler classes.

---

## 📁 Project Structure

```
DotnetCqrs/
├── Features/
│   └── Users/
│       ├── Controller.cs        # API endpoints for User
│       ├── Command.cs           # Write request model (e.g. CreateUserCommand)
│       ├── Query.cs             # Read request model (e.g. GetUsersQuery)
│       ├── Request.cs           # DTOs for create/update requests
│       ├── Entity.cs            # User entity and EF config
│       ├── Repository.cs        # User repository interface/impl
│       └── Handlers/
│           ├── Create.cs        # Handles user creation logic
│           ├── Gets.cs          # Handles getting user list logic
│           ├── Get.cs           # Handles getting single user
│           ├── Update.cs        # Handles user update
│           └── Delete.cs        # Handles user deletion
├── Infrastructure/
│   └── Data/
│       ├── DbContext.cs         # EF Core database context
│       ├── Repository.cs        # Basic repository layer
│       ├── Entity.cs            # Base entity classes
│       ├── DateTimeFormat.cs    # Custom DateTime JSON converters
│       ├── TimeFormat.cs        # Custom TimeOnly JSON converters
│       └── ExcelService.cs      # Excel import/export helpers
│
├── Program.cs                   # Main entry point and DI config
├── compose.yaml                 # Docker Compose file for PostgreSQL
├── Dockerfile                   # Dockerfile for app container
└── Migrations/                  # EF Core migrations
```

---

## 🔄 Code Flow (Manual CQRS)

```
Client (API Call)
   |
   v
UsersController
   |
   v
Handler Class
   |
   v
Repository / AppDbContext
   |
   v
Database
   |
   v
(Returns data up the chain)
```


**Step-by-step Explanation (CRUD Example for User):**

1. **Client (API Call):**  
   The client (web frontend, Postman, etc.) sends an HTTP request to the API endpoint, such as:
   - `GET /users` (Read all users)
   - `GET /users/{id}` (Read a single user)
   - `POST /users` (Create a new user)
   - `PUT /users/{id}` (Update an existing user)
   - `DELETE /users/{id}` (Delete a user)

2. **UsersController:**  
   The ASP.NET Core controller receives the HTTP request.  
   - For each CRUD operation, the controller parses the request and constructs a Command or Query object.
   - It calls the corresponding Handler class, passing the Command/Query.

3. **Handler Class:**  
   The Handler (e.g., `GetAllUsersHandler`, `GetUserByIdHandler`, `CreateUserHandler`, `UpdateUserHandler`, `DeleteUserHandler`) contains the business logic for the operation:
   - **Read:** Handlers fetch user data from the repository.
   - **Create:** Handler validates and creates a new user entity.
   - **Update:** Handler fetches, modifies, and saves the user entity.
   - **Delete:** Handler removes the user entity.
   - Each handler interacts with the Repository or DbContext.

4. **Repository / AppDbContext:**  
   The Repository abstracts data access and interacts with the Entity Framework Core `AppDbContext`.  
   - Handles querying, adding, updating, or deleting user entities.
   - Keeps data access logic separate from business logic.

5. **Database:**  
   The `AppDbContext` translates LINQ queries and commands into SQL, executed against the PostgreSQL database.  
   - Data is persisted or retrieved as needed.
   - Results are returned up the chain to the client.

**Summary:**  
Each user CRUD API request flows through Controller → Handler → Repository/DbContext → Database, then back up with the response.  
This separation ensures clear responsibility for each layer and makes the codebase easy to maintain and extend.

- Controllers manually call Handler classes (no MediatR).
- Handlers contain single-purpose logic: either a Command or a Query.
- `DbContext` or `Repository` is used for data access.

---

## 🚀 Getting Started

### Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Docker (for PostgreSQL)
- PostgreSQL connection string (or use docker-compose)

### Run Locally

```bash
git clone https://github.com/stonehappi/dotnet-cqrs.git
cd dotnet-cqrs/DotnetCqrs
dotnet run
```

### Run with Docker

```bash
docker compose up --build
```

Visit Swagger UI: `http://localhost:8081/swagger`

---

## 💬 Example: Getting All Users

```csharp
[HttpGet]
public async Task<IActionResult> GetAll()
{
    var users = await getAllUsersHandler.HandleAsync(new GetAllUsersQuery());
    return Ok(users);
}
```

- Controller receives the HTTP request and calls the handler.
- Handler queries the repository and returns the result.

---

## 🧪 Test CRUD Example Data

Here are example JSON payloads for testing the User CRUD API endpoints:

### Create User (`POST /users`)

```json
{
  "username": "alice",
  "email": "alice@example.com",
  "startDate": "2024-05-01T09:00:00Z",
  "startTime": "09:00:00"
}
```

### Update User (`PUT /users/1`)

```json
{
  "username": "alice-updated",
  "email": "alice.new@example.com",
  "startDate": "2024-05-02T10:00:00Z",
  "startTime": "10:00:00"
}
```

### Example Response for Get User (`GET /users/1`)

```json
{
  "id": 1,
  "username": "alice",
  "email": "alice@example.com",
  "startDate": "2024-05-01T09:00:00Z",
  "startTime": "09:00:00"
}
```

### Example Response for Get All Users (`GET /users`)

```json
[
  {
    "id": 1,
    "username": "alice",
    "email": "alice@example.com",
    "startDate": "2024-05-01T09:00:00Z",
    "startTime": "09:00:00"
  },
  {
    "id": 2,
    "username": "bob",
    "email": "bob@example.com",
    "startDate": "2024-05-03T08:30:00Z",
    "startTime": "08:30:00"
  }
]
```

### Delete User

- No body required for `DELETE /users/{id}`.
- Returns HTTP 204 No Content on success.

---

## 📝 Features

- Manual CQRS pattern (no MediatR)
- Clean separation of Commands, Queries, Handlers, and Entities
- EF Core with PostgreSQL
- Swagger UI for API docs
- Docker support for easy local development

---

## 📚 Learn More

- [CQRS Pattern - Microsoft Docs](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [EF Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Web API Docs](https://learn.microsoft.com/en-us/aspnet/core/web-api/)

---

## 📄 License

MIT

---

> Created with 💡 by [stonehappi](https://github.com/stonehappi)
