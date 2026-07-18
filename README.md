# Task Management System API

## 📖 Overview
The Task Management System API is a robust backend service designed to manage tasks efficiently within teams or for individual users. It allows users to create, update, assign, and track tasks with full support for advanced filtering, pagination, sorting, and Role-Based Access Control (RBAC).

This project demonstrates real-world backend engineering practices, focusing on clean architecture, secure authentication, and scalable data handling.

---

## ✨ Core Features

### 🔐 Authentication & Authorization
* **JWT-based Authentication:** Secure user login and registration.
* **Role-Based Access Control (RBAC):**
  * `Admin`: Full control over all tasks and users.
  * `Manager`: Can view all tasks, modify assigned tasks, and view users.
  * `User`: Can only view and update tasks assigned directly to them.

### 👥 User Management
* Complete CRUD operations for users (Admin restricted).
* Profile updates and user retrieval based on assigned roles.

### 📋 Task Management
* Comprehensive task lifecycle management (ToDo ➡️ InProgress ➡️ Done).
* Task metadata including Priority (Low, Medium, High) and Due Dates.
* Strict business logic (e.g., tasks cannot skip to "Done" without being "InProgress", due dates cannot be in the past).

### 🔍 Advanced Data Shaping
* **Filtering:** By Status, Priority, Assigned User, and Due Date range.
* **Pagination:** Configurable `PageNumber` and `PageSize`.
* **Sorting:** Sort results by `DueDate`, `CreatedAt`, or `Priority`.

---

## 🏗️ Architecture
This API is built using a **Layered (N-Tier) Architecture** to ensure separation of concerns and maintainability:

* **Controllers:** Handle HTTP requests and responses.
* **Services:** Contain core business logic and rules.
* **Repositories:** Manage data access and persistence.
* **Models (Entities):** Represent database schemas.
* **DTOs (Data Transfer Objects):** Shape data for API requests and responses.

---

## 🗄️ Database Design

### `Users` Table
| Column | Type | Description |
| :--- | :--- | :--- |
| `Id` | PK | Unique identifier |
| `Name` | String | User's full name |
| `Email` | String | User's email address |
| `PasswordHash` | String | Hashed password |
| `Role` | Enum/String | Admin, Manager, or User |

### `Tasks` Table
| Column | Type | Description |
| :--- | :--- | :--- |
| `Id` | PK | Unique identifier |
| `Title` | String | Task title |
| `Description` | String | Detailed task description |
| `Status` | Enum | ToDo, InProgress, Done |
| `Priority` | Enum | Low, Medium, High |
| `DueDate` | DateTime | Deadline for the task |
| `CreatedAt` | DateTime | Task creation timestamp |
| `AssignedTo` | FK | Relates to `Users.Id` |
| `CreatedBy` | FK | Relates to `Users.Id` |

---

## 🚀 API Endpoints

### Auth
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login` | Authenticate and get JWT |

### Users
| Method | Endpoint | Description | Access Level |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/users` | Get all users | Admin / Manager |
| `GET` | `/api/users/{id}` | Get user by ID | Authenticated |
| `PUT` | `/api/users/{id}` | Update user profile | Authenticated |
| `DELETE` | `/api/users/{id}` | Delete a user | Admin |

### Tasks
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/tasks` | Create a new task |
| `GET` | `/api/tasks` | Get all tasks (supports pagination/filtering) |
| `GET` | `/api/tasks/{id}` | Get task by ID |
| `PUT` | `/api/tasks/{id}` | Update an existing task |
| `DELETE`| `/api/tasks/{id}` | Delete a task |

* **Example Paginated/Filtered Request:** 
  `GET /api/tasks?pageNumber=1&pageSize=10&status=InProgress&priority=High`

---

## 🛡️ Validation & Error Handling
* **Input Validation:** Enforced via DTO validation (e.g., FluentValidation / Data Annotations).
* **Global Error Handling:** A centralized middleware catches unhandled exceptions and returns standardized JSON responses.
* **Standard HTTP Codes:**
  * `200 OK` - Request successful.
  * `201 Created` - Resource created successfully.
  * `400 Bad Request` - Invalid input or business rule violation.
  * `401 Unauthorized` - Missing or invalid JWT token.
  * `403 Forbidden` - Insufficient role permissions.
  * `404 Not Found` - Resource does not exist.

---

## 📈 Future Enhancements (Roadmap)
- [ ] **Logging:** Implement structured logging (e.g., Serilog).
- [ ] **Testing:** Add extensive Unit Testing (e.g., xUnit, NUnit, Moq).
- [ ] **API Documentation:** Integrate Swagger UI for seamless API testing and exploration.
- [ ] **Caching:** Implement caching strategies (Memory Cache / Redis) for task lists.
- [ ] **Soft Deletion:** Implement soft-delete logic instead of hard database row removal.
