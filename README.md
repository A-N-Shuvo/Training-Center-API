# Training Center API

A comprehensive ASP.NET Core Web API for managing training center operations, including student enrollment, course management, batch scheduling, and payment processing.

## 🚀 Features

- **Student Management** - Complete student profiles with enrollment tracking
- **Course Catalog** - Course management with pricing and duration
- **Batch Scheduling** - Flexible batch creation with timing and capacity
- **Payment System** - Payment tracking with installment support
- **Attendance System** - Daily attendance recording and monitoring
- **Exam Management** - Exam scheduling and result tracking
- **User Authentication** - Secure user account management

## 🏗️ Project Structure

### Models
- **Student** - Student personal and academic information
- **Course** - Course details, duration, and pricing
- **Batch** - Training batches with schedules and capacity
- **Payment** - Payment records and installment tracking
- **Attendance** - Daily attendance records
- **Exam** - Exam scheduling and results
- **User** - User authentication and profiles

### Controllers
- **StudentsController** - CRUD operations for student management
- **CoursesController** - Course catalog and management
- **BatchesController** - Batch scheduling and enrollment
- **PaymentsController** - Payment processing and tracking
- **AttendanceController** - Attendance recording and reports
- **ExamsController** - Exam management and results
- **UsersController** - User authentication and account management

## 💻 Technology Stack

- **Framework**: ASP.NET Core Web API
- **Database**: Entity Framework Core (SQL Server)
- **Authentication**: JWT Bearer Token
- **Validation**: Data Annotations
- **Architecture**: MVC Pattern

## 🔧 API Endpoints

### Students
- `GET /api/Students` - Get all students
- `GET /api/Students/{id}` - Get student by ID
- `POST /api/Students` - Create new student
- `PUT /api/Students/{id}` - Update student
- `DELETE /api/Students/{id}` - Delete student

### Courses
- `GET /api/Courses` - Get all courses
- `GET /api/Courses/{id}` - Get course by ID
- `POST /api/Courses` - Create new course
- `PUT /api/Courses/{id}` - Update course

### Payments
- `GET /api/Payments` - Get all payments
- `POST /api/Payments` - Create payment record
- `GET /api/Payments/student/{studentId}` - Get payments by student

### Attendance
- `POST /api/Attendance` - Mark attendance
- `GET /api/Attendance/batch/{batchId}` - Get attendance by batch

## 🚀 Getting Started

### Prerequisites
- .NET 6.0 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Configure database connection in `appsettings.json`
3. Run database migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

## 📊 Database Schema

The API uses a relational database with tables for:
- Students (Personal info, contact details, enrollment status)
- Courses (Course details, pricing, duration)
- Batches (Schedule, timing, capacity limits)
- Payments (Payment records, installments, due dates)
- Attendance (Daily records, status tracking)
- Exams (Exam schedules, results, grading)

## 🔐 Authentication

The API supports JWT-based authentication with:
- User registration and login
- Role-based authorization
- Secure password hashing
- Token expiration management

## 📈 Key Functionalities

- **Student Enrollment**: Complete enrollment process with course selection
- **Batch Management**: Create and manage training batches with capacity limits
- **Payment Tracking**: Monitor payments and dues with installment support
- **Attendance System**: Daily attendance marking and reporting
- **Exam Management**: Schedule exams and record results
- **Reporting**: Generate various reports for administration

## 🤝 Contributing

Feel free to contribute to this project by submitting issues, feature requests, or pull requests.

