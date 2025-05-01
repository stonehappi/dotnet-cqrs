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

> **Environment Variables:**  
> The code uses environment variables for configuration, such as:
> - `DB_CONNECTION` (PostgreSQL connection string)
> - `TIME_FORMAT` (for time serialization)
> - `DATETIME_FORMAT` (for datetime serialization)
> - `TZ` (timezone, e.g., `Asia/Phnom_Penh`)
>
> You can set these in your shell before running, or use a `.env` file with your preferred values.  
> If not set, the code uses sensible defaults.

```bash
git clone https://github.com/stonehappi/dotnet-cqrs.git
cd dotnet-cqrs/DotnetCqrs
# Example: set environment variables (optional)
export DB_CONNECTION="Server=localhost:5432;Database=cqrs;User Id=postgres;Password=password;"
export TZ="Asia/Phnom_Penh"
dotnet run
```

### Run with Docker

```bash
docker compose up --build
```

Visit Swagger UI: `http://localhost:8081/swagger`

### Docker and Docker Compose Explained

**Dockerfile:**  
- Multi-stage build for efficient, small runtime images.
- Uses .NET 9 SDK to build and publish the app, then runs it on the .NET 9 ASP.NET runtime.
- Exposes ports 8080 and 8081 (the app listens on 8080 by default).
- Uses `USER $APP_UID` if set, for running as a non-root user.

**compose.yaml:**  
- Defines a `dotnetcqrs` service that builds the image using the Dockerfile.
- Maps host port 8081 to container port 8080 (`"8081:8080"`).
- Sets environment variables:
  - `TZ` (timezone, e.g., `America/Los_Angeles`)
  - `DB_CONNECTION` (PostgreSQL connection string)
- Connects to an external Docker network named `docker_postgres` (make sure this exists and your PostgreSQL is accessible there).

**Customizing:**  
- Change `TZ` or `DB_CONNECTION` in `compose.yaml` to match your environment.
- If you want to use a different port, adjust the `ports` mapping.
- If you want to run as a specific user, set `APP_UID` in your compose or Docker run command.

**Usage:**  
- To build and run with Docker Compose:  
  ```bash
  docker compose up --build
  ```
- The app will be available at [http://localhost:8081/swagger](http://localhost:8081/swagger).

---

## 💬 Example: Getting All Users

```csharp
// Controller action
[HttpGet]
public async Task<IActionResult> GetAll()
{
    // Step 1: Create the query object
    var query = new GetAllUsersQuery();

    // Step 2: Call the handler with the query
    var users = await getAllUsersHandler.HandleAsync(query);

    // Step 3: Return the result
    return Ok(users);
}
```

- Controller receives the HTTP request and calls the handler.
- The handler processes the query and returns the user list.

**Handler Implementation:**

```csharp
// filepath: DotnetCqrs/Features/Users/Handlers/Gets.cs
public class GetAllUsersHandler(IUsersRepository repo) : IGetAllUsersHandler
{
    public async Task<IEnumerable<UsersEntity>> HandleAsync(GetAllUsersQuery query)
    {
        var users = await repo.GetAll()
            .ToListAsync();
        return users;
    }
}
```

**Repository Method:**

```csharp
// filepath: DotnetCqrs/Infrastructure/Data/Repository.cs
public virtual IQueryable<T> GetAll()
{
    return _dbSet;
}
```

---

## 💬 Example: Getting a User by Id

```csharp
// Controller action
[HttpGet("{id:int}")]
public async Task<IActionResult> GetById(int id)
{
    // Step 1: Create the query object
    var query = new GetUserByIdQuery(id);

    // Step 2: Call the handler with the query
    var user = await getHandler.HandleAsync(query);

    // Step 3: Return the result
    return Ok(user);
}
```

**Handler Implementation:**

```csharp
// filepath: DotnetCqrs/Features/Users/Handlers/Get.cs
public class GetUserByIdHandler(IUsersRepository repo) : IGetUserByIdHandler
{
    public async Task<UsersEntity> HandleAsync(GetUserByIdQuery query)
    {
        var user = await repo.GetSingleAsync(e => e.Id == query.Id);
        if (user == null) throw new NotFoundEx();
        return user;
    }
}
```

---

## 💬 Example: Creating a User

```csharp
// Controller action
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
{
    // Step 1: Create the command object
    var command = new CreateUserCommand(request.Username, request.Email, request.StartDate, request.StartTime);

    // Step 2: Call the handler with the command
    await createHandler.HandleAsync(command);

    // Step 3: Return the result
    return NoContent();
}
```

**Handler Implementation:**

```csharp
// filepath: DotnetCqrs/Features/Users/Handlers/Create.cs
public class CreateUserHandler(IUsersRepository repo) : ICreateUserHandler
{
    public async Task HandleAsync(CreateUserCommand command)
    {
        var user = new UsersEntity
        {
            Username = command.Username,
            Email = command.Email,
            StartDate = command.StartDate,
            StartTime = command.StartTime
        };
        await repo.AddAsync(user);
        await repo.CommitAsync();
    }
}
```

---

## 💬 Example: Updating a User

```csharp
// Controller action
[HttpPut("{id:int}")]
public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
{
    // Step 1: Create the command object
    var command = new UpdateUserCommand(id, request.Username, request.Email, request.StartDate, request.StartTime);

    // Step 2: Call the handler with the command
    await updateHandler.HandleAsync(command);

    // Step 3: Return the result
    return NoContent();
}
```

**Handler Implementation:**

```csharp
// filepath: DotnetCqrs/Features/Users/Handlers/Update.cs
public class UpdateUserHandler(IUsersRepository repo) : IUpdateUserHandler
{
    public async Task HandleAsync(UpdateUserCommand command)
    {
        var user = await repo.GetSingleAsync(e => e.Id == command.Id);
        if (user == null) throw new NotFoundEx();
        user.Username = command.Username;
        user.Email = command.Email;
        user.StartDate = command.StartDate;
        user.StartTime = command.StartTime;
        repo.Update(user);
        await repo.CommitAsync();
    }
}
```

---

## 💬 Example: Deleting a User

```csharp
// Controller action
[HttpDelete("{id:int}")]
public async Task<IActionResult> Delete(int id)
{
    // Step 1: Call the handler with the id
    await deleteHandler.HandleAsync(id);

    // Step 2: Return the result
    return NoContent();
}
```

**Handler Implementation:**

```csharp
// filepath: DotnetCqrs/Features/Users/Handlers/Delete.cs
public class DeleteUserHandler(IUsersRepository repo) : IDeleteUserHandler
{
    public async Task HandleAsync(int id)
    {
        var user = await repo.GetSingleAsync(e => e.Id == id);
        if (user == null) throw new NotFoundEx();
        repo.Remove(user);
        await repo.CommitAsync();
    }
}
```

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
