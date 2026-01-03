# TaskMaster - Task Management System

A comprehensive, production-grade task management API built with .NET 10, designed to streamline workflows and enhance productivity for ABC Corp.

## 🚀 Features

### Core Functionality
- ✅ **User Authentication & Authorization**: JWT-based authentication with role-based access control
- ✅ **Task Management**: Complete CRUD operations for tasks
- ✅ **Task Assignment**: Assign tasks to team members with email notifications
- ✅ **Task Tracking**: Track progress with comments and activity logs
- ✅ **Task Prioritization**: LOW, MEDIUM, HIGH priority levels
- ✅ **Status Management**: PENDING, IN_PROGRESS, COMPLETED statuses
- ✅ **Advanced Filtering**: Filter by status, priority, due date with pagination
- ✅ **Search Functionality**: Full-text search across task titles and descriptions

### Collaboration Features
- ✅ **Comments System**: Team discussions within tasks
- ✅ **Activity Logging**: Complete audit trail of task changes
- ✅ **Email Notifications**: Automated notifications for assignments and deadlines

### Advanced Features
- ✅ **Deadline Management**: Automatic reminders for due and overdue tasks
- ✅ **Background Services**: Scheduled notifications via hosted service
- ✅ **Health Checks**: Monitor application and database health
- ✅ **API Documentation**: Interactive Swagger/OpenAPI documentation

## 🏗️ Architecture

### Technology Stack
- **Framework**: .NET 10.0
- **Database**: PostgreSQL 16
- **ORM**: Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **Containerization**: Docker & Docker Compose

### Design Patterns
- Repository Pattern for data access
- Dependency Injection for loose coupling
- Service Layer for business logic
- Middleware for cross-cutting concerns
- Background Services for scheduled tasks

## 📦 Project Structure

```
TaskMaster/
├── Controllers/          # API endpoints
├── Services/            # Business logic
├── Repositories/        # Data access layer
├── Models/             # Domain entities
├── DTOs/               # Data transfer objects
├── Data/               # Database context
├── Middlewares/        # Custom middleware
├── Configuration/      # App settings models
├── Migrations/         # EF Core migrations
├── Dockerfile          # Docker configuration
├── docker-compose.yml  # Multi-container setup
└── Program.cs         # Application entry point
```

## 🚀 Getting Started

### Prerequisites
- [Docker](https://www.docker.com/get-started) (recommended)
- OR [.NET 8 SDK](https://dotnet.microsoft.com/download) + [PostgreSQL 16](https://www.postgresql.org/download/)

### Quick Start with Docker (Recommended)

1. **Clone the repository**
```bash
git clone <repository-url>
cd TaskMaster
```

2. **Configure environment**
```bash
# Linux/Mac
cp .env.example .env

# Windows
copy .env.example .env
```

3. **Update .env file** with your SMTP credentials for email notifications

4. **Build and start the application**

**Linux/Mac:**
```bash
chmod +x build.sh start.sh stop.sh
./build.sh
./start.sh
```

**Windows:**
```batch
build.bat
start.bat
```

5. **Access the application**
- API: http://localhost:8080
- Swagger UI: http://localhost:8080
- PgAdmin: http://localhost:5050

### Manual Setup (Without Docker)

1. **Install PostgreSQL** and create database:
```sql
CREATE DATABASE taskmaster;
```

2. **Update appsettings.json** with your connection string

3. **Run migrations**
```bash
dotnet ef database update
```

4. **Run the application**
```bash
dotnet run
```

## 📚 API Documentation

### Authentication Endpoints

#### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

### Task Endpoints

All task endpoints require authentication. Include the JWT token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

#### Create Task
```http
POST /api/tasks
Content-Type: application/json

{
  "title": "Implement new feature",
  "description": "Add user profile page",
  "dueDate": "2025-12-31T23:59:59Z",
  "priority": "HIGH",
  "status": "PENDING"
}
```

#### Get All Tasks
```http
GET /api/tasks
```

#### Get Task by ID
```http
GET /api/tasks/{id}
```

#### Update Task
```http
PUT /api/tasks/{id}
Content-Type: application/json

{
  "title": "Updated title",
  "status": "IN_PROGRESS"
}
```

#### Delete Task
```http
DELETE /api/tasks/{id}
```

#### Assign Task (Admin only)
```http
PUT /api/tasks/{id}/assign
Content-Type: application/json

{
  "userId": "user-guid-here"
}
```

#### Filter Tasks
```http
GET /api/tasks/filter?status=IN_PROGRESS&priority=HIGH&page=1&pageSize=10
```

#### Search Tasks
```http
GET /api/tasks/search?q=feature&myTasks=true
```

### Comment Endpoints

#### Add Comment
```http
POST /api/tasks/{taskId}/comments
Content-Type: application/json

{
  "content": "This is a comment"
}
```

#### Get Comments
```http
GET /api/tasks/{taskId}/comments
```

### Activity Log Endpoints

#### Get Activity Logs
```http
GET /api/tasks/{taskId}/activity
```

### User Endpoints

#### Get All Users
```http
GET /api/users
```

#### Get User by ID
```http
GET /api/users/{id}
```

## 🔧 Configuration

### Environment Variables

Key configuration options in `.env`:

```bash
# Database
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=taskmaster

# SMTP (for email notifications)
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password

# JWT
JWT_KEY=your-secret-key-min-32-chars

# CORS
ALLOWED_ORIGINS=http://localhost:3000
```

### Email Configuration

For Gmail SMTP:
1. Enable 2-factor authentication
2. Generate an [App Password](https://support.google.com/accounts/answer/185833)
3. Use the app password in `SMTP_PASSWORD`

## 🧪 Testing

Access Swagger UI at http://localhost:8080 to test all endpoints interactively.

### Sample Test Flow

1. Register a new user
2. Login to get JWT token
3. Click "Authorize" in Swagger and enter: `Bearer <your-token>`
4. Create tasks, add comments, and test all features

## 🔒 Security

- Passwords hashed using BCrypt
- JWT tokens with configurable expiration
- Role-based authorization (USER, ADMIN)
- CORS configured for frontend integration
- SQL injection protection via EF Core parameterized queries
- Input validation on all endpoints

## 📊 Database Schema

### Users
- Id (Guid, PK)
- Email (unique)
- PasswordHash
- Role
- CreatedAt

### Tasks
- Id (Guid, PK)
- Title
- Description
- DueDate
- Priority (LOW, MEDIUM, HIGH)
- Status (PENDING, IN_PROGRESS, COMPLETED)
- AssignedToUserId (FK)
- CreatedByUserId (FK)
- CreatedAt

### TaskComments
- Id (Guid, PK)
- TaskItemId (FK)
- UserId (FK)
- Content
- CreatedAt

### ActivityLogs
- Id (Guid, PK)
- TaskItemId (FK)
- UserId (FK)
- Action
- Description
- CreatedAt

## 🛠️ Management Commands

### Docker Commands

```bash
# View logs
docker-compose logs -f api

# Restart services
docker-compose restart

# Stop and remove all containers
docker-compose down

# Stop and remove including volumes (database data)
docker-compose down -v

# Rebuild containers
docker-compose up -d --build
```

### Database Commands

```bash
# Access PostgreSQL via PgAdmin
# Navigate to http://localhost:5050
# Login: admin@taskmaster.com / admin

# Or via command line
docker exec -it taskmaster-db psql -U postgres -d taskmaster
```

## 📈 Monitoring

### Health Check
```http
GET /health
```

Returns database connectivity status and application health.

### Logging

Application logs are available via:
```bash
docker-compose logs -f api
```

## 🤝 Contributing

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Commit changes: `git commit -m 'feat: add amazing feature'`
3. Push to branch: `git push origin feature/your-feature`
4. Open a Pull Request

### Commit Message Convention

- `feat:` New feature
- `fix:` Bug fix
- `docs:` Documentation changes
- `refactor:` Code refactoring
- `test:` Adding tests
- `chore:` Maintenance tasks

## 📝 License

None
## 👥 Support

For issues and questions:
- Email: amponsem_michael@yahoo.com

## 🗺️ Roadmap

### Upcoming Features
- [ ] Real-time notifications
- [ ] File attachments for tasks
- [ ] Calendar integration
- [ ] Task templates
- [ ] Advanced reporting
- [ ] Mobile app

---
