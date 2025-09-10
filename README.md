# Municipal Report Management System

A comprehensive web application for managing municipal reports with user and administrator interfaces. This system allows citizens to submit reports about municipal issues and enables administrators to review, manage, and track the status of these reports.

## Table of Contents
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Installation](#installation)
- [Database Schema](#database-schema)
- [Project Structure](#project-structure)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Screenshots](#screenshots)
- [Admin Login](#admin-login)

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

## Data Structure
### HashSet
<img width="1004" height="534" alt="image" src="https://github.com/user-attachments/assets/e2e4a359-c5a1-49d5-ae7d-b576d2dca8ea" />

### Dictionary
<img width="1035" height="541" alt="image" src="https://github.com/user-attachments/assets/f4f4308b-06ab-4e73-8af8-a7bba128502f" />

### Database
<img width="1139" height="1159" alt="image" src="https://github.com/user-attachments/assets/a63c8333-99f2-4ae6-9228-9ecee2769efc" />

### IQueryable
<img width="923" height="836" alt="image" src="https://github.com/user-attachments/assets/13080132-2050-4b36-93a0-8d47dfbe4f94" />

### IEnumerable
<img width="446" height="120" alt="image" src="https://github.com/user-attachments/assets/12be66f9-8bf5-4117-a4fe-00328afd52a2" />
<img width="1501" height="634" alt="image" src="https://github.com/user-attachments/assets/74a74470-82c8-4c08-9197-eda48b2a6047" />

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
<img width="2493" height="1278" alt="image" src="https://github.com/user-attachments/assets/ed4dc695-68c5-4c4f-bbc5-2a300929ba48" />

2. **Login and Sign Upload**
<img width="2513" height="1278" alt="image" src="https://github.com/user-attachments/assets/6a9bc7cb-3fac-46b3-87d9-fb1404f362cd" />
<img width="2498" height="1221" alt="image" src="https://github.com/user-attachments/assets/d160f3d9-c2c2-492b-80c9-1f2edb656ca4" />

3. **User Dashboard**
<img width="2503" height="1220" alt="image" src="https://github.com/user-attachments/assets/a0d40b18-0b4d-4c2e-ac0d-bcf1c1958c47" />


### Admin Interface
1. **Admin Dashboard**
<img width="2497" height="1222" alt="image" src="https://github.com/user-attachments/assets/426144a5-651c-4ac9-8fbb-93fb2adf7ddc" />

2. **Report Management**
<img width="2496" height="1131" alt="image" src="https://github.com/user-attachments/assets/c9f6dcba-d7dd-4fb4-8096-a3b8da5da097" />
<img width="2514" height="1130" alt="image" src="https://github.com/user-attachments/assets/1c82cf7b-5355-49fc-b0c9-287c81220886" />

## Admin Login
- admin@example.com
- Admin@123
