# Municipal Report Management System

A comprehensive web application for managing municipal reports with user and administrator interfaces. This system allows citizens to submit reports about municipal issues and enables administrators to review, manage, and track the status of these reports.

## Table of Contents
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database Schema](#database-schema)
- [Project Structure](#project-structure)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Screenshots](#screenshots)
- [Contributing](#contributing)
- [License](#license)

## Features

### User Features
- User registration and authentication
- Submit new reports with details and location
- View personal report history
- Track report status
- Update profile information

### Admin Features
- Dashboard with report statistics
- View all reports with filtering options
- Review and update report status
- Manage user accounts
- Export report data

## Technology Stack

### Backend
- **Framework**: ASP.NET Core
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity

### Frontend
- **UI Framework**: Bootstrap 5
- **Client-side Scripting**: JavaScript, jQuery
- **Charts**: Chart.js (for admin dashboard)
- **Maps**: (If applicable, e.g., Google Maps API for location selection)

## Installation

### Prerequisites
- .NET 7.0 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 (recommended) or VS Code

### Setup Instructions
1. Clone the repository:
   ```bash
   git clone [repository-url]
   ```

2. Navigate to the project directory:
   ```bash
   cd Municiple_Project_st10259527
   ```

3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

4. Update the connection string in `appsettings.json` to point to your SQL Server instance.

5. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

6. Run the application:
   ```bash
   dotnet run
   ```

## Configuration

### Environment Variables
Create a `appsettings.Development.json` file in the project root with the following content:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=MunicipalReports;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## Database Schema

### Users Table
- UserId (PK)
- Username
- Email
- PasswordHash
- FirstName
- LastName
- Role
- CreatedAt
- LastLogin

### Reports Table
- ReportId (PK)
- UserId (FK)
- ReportType
- Description
- ReportDate
- Location
- Status (Pending/InReview/Approved/Rejected/Completed)
- UpdatedAt
- (Additional fields as per your model)

## Project Structure

```
Municiple_Project_st10259527/
├── Controllers/         # MVC Controllers
├── Models/              # Data models
├── Views/               # Razor views
├── Services/            # Business logic and services
├── Repository/          # Data access layer
├── Attributes/          # Custom attributes
├── ViewModels/          # View-specific models
├── wwwroot/             # Static files
└── Migrations/          # Entity Framework migrations
```

## Usage

1. **User Registration**: New users can register through the registration page.
2. **Report Submission**: Logged-in users can submit new reports with details and location.
3. **Report Management**: Users can view their report history and status.
4. **Admin Dashboard**: Administrators can manage all reports and user accounts.

## API Endpoints

### User Endpoints
- `GET /User/Register` - Display registration form
- `POST /User/Register` - Create new user account
- `GET /User/Login` - Display login form
- `POST /User/Login` - Authenticate user
- `GET /User/Profile` - View user profile
- `POST /User/Profile` - Update user profile

### Report Endpoints
- `GET /Report` - List user's reports
- `GET /Report/Create` - Display report creation form
- `POST /Report/Create` - Submit new report
- `GET /Report/Details/{id}` - View report details
- `GET /Report/Edit/{id}` - Edit report
- `POST /Report/Edit/{id}` - Update report

### Admin Endpoints
- `GET /Admin/Dashboard` - Admin dashboard with statistics
- `GET /Admin/Reports` - List all reports
- `GET /Admin/Report/{id}` - View report details (admin)
- `POST /Admin/Report/UpdateStatus` - Update report status
- `GET /Admin/Users` - List all users
- `POST /Admin/User/UpdateRole` - Update user role

## Screenshots

### User Interface
<!-- Add your screenshots here -->
1. **Home Page**
   ![Home Page](screenshots/home-page.png)

2. **Report Submission**
   ![Report Submission](screenshots/report-submission.png)

3. **User Dashboard**
   ![User Dashboard](screenshots/user-dashboard.png)

### Admin Interface
1. **Admin Dashboard**
   ![Admin Dashboard](screenshots/admin-dashboard.png)

2. **Report Management**
   ![Report Management](screenshots/report-management.png)

3. **User Management**
   ![User Management](screenshots/user-management.png)

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

*Note: Replace `[repository-url]` with your actual repository URL and add your screenshots to the `screenshots` directory.*
