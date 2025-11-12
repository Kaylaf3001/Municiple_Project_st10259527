# Municipal Report Management System

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [System Architecture](#system-architecture)
- [Project Structure](#project-structure)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [Database Seeding](#database-seeding)
- [Running the Application](#running-the-application)
- [Usage Guide](#usage-guide)
- [Admin Guide](#admin-guide)
- [Troubleshooting & FAQ](#troubleshooting--faq)

## Overview
This web application enables municipalities to efficiently manage service requests, community events, public announcements, and citizen reports. It supports both citizen and admin roles, providing a seamless experience for issue reporting, event participation, and administration.

## Features
- **User Authentication**: Secure login/sign-up for citizens and admins.
- **Service Requests**: Citizens submit requests; system auto-infers priority/category; track status via unique code.
- **Event Management**: Browse, search, filter, and group events by date/category. Admins create/manage events.
- **Announcements**: View and manage official municipal announcements.
- **Reporting**: Submit and review reports on issues; admins update statuses and provide feedback.
- **Personalized Recommendations**: Event suggestions based on user search history.
- **Session Management**: Automatic session timeout for security.
- **Advanced Data Structures**: Uses trees, heaps, graphs, and queues for efficient data handling.

## System Architecture
- **ASP.NET Core MVC**: Implements Model-View-Controller for separation of concerns.
- **Entity Framework Core**: ORM for database access and migrations.
- **SQLite**: Lightweight, file-based relational database.
- **Session & Dependency Injection**: For secure, scalable, and maintainable code.
- **Custom Data Structures**: For optimized searching, recommendations, and request/event management.

## Project Structure
- `Controllers/`: Handles HTTP requests, user/admin flows.
- `Models/`: Data models for users, events, reports, announcements, service requests.
- `Repository/`: Data access abstraction and implementations.
- `Services/`: Core logic, business rules, data structure implementations.
- `ViewModels/`: Data passed between controllers and views.
- `Views/`: Razor pages for UI (user/admin dashboards, forms, etc.).
- `Data/`: Database context and seed data logic.
- `wwwroot/`: Static files (CSS, JS, images).

## Technologies Used
- **.NET 8.0** (TargetFramework)
- **ASP.NET Core MVC**
- **Entity Framework Core 9.0.9** (with SQLite & SQL Server providers)
- **Razor Views**
- **Bootstrap** (UI styling)
- **Session Management** (ASP.NET Core)
- **Custom Data Structures** (trees, heaps, graphs, queues)

## Prerequisites
- [.NET 8.0 SDK or later](https://dotnet.microsoft.com/download)
- [SQLite](https://www.sqlite.org/download.html) (optional; auto-created on first run)
- Git

## Installation & Setup
1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd Municipal_Project
   ```
2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```
3. **Build the project:**
   ```bash
   dotnet build
   ```

## Configuration
- **Database Connection:**
  - Default: `Data/municipal.db` (see `appsettings.json`)
  - You can adjust connection strings in `appsettings.json` for other environments.
- **Session Settings:**
  - Configured in `Program.cs` for 30-minute idle timeout.
- **Logging:**
  - Levels set in `appsettings.json`.

## Database Seeding
- On first run, the database is seeded with sample users (admin/citizens), events, and announcements (see `Data/SeedData.cs`).
- To re-seed, delete `municipal.db` and restart the application.

## Running the Application
1. **Apply migrations and seed data:**
   ```bash
   dotnet ef database update
   ```
2. **Run the application:**
   ```bash
   dotnet run
   ```
   - Access at `https://localhost:5001` (or as set in `launchSettings.json`).

## Usage Guide
### For Citizens
- **Register/Login:** Use the sign-up and login forms.
- **Submit Service Requests:** Fill in title, description, category, and location. Priority/category are inferred automatically.
- **Track Requests:** Use your unique tracking code to view status.
- **View Events/Announcements:** Browse upcoming events, filter by date/category, and read announcements.
- **Get Recommendations:** Personalized event suggestions appear on your dashboard.

### Example Citizen Workflow
1. Register an account.
2. Submit a service request (e.g., "Broken streetlight at Main St.").
3. Receive tracking code and monitor request status.
4. View and sign up for upcoming community events.
5. Check announcements for important updates.

## Admin Guide
- **Login:** Use pre-seeded admin credentials or register as an admin.
- **Dashboard:** View analytics, recent activity, and quick links.
- **Manage Service Requests:** Review, filter, and update statuses; assign priorities/categories if needed.
- **Manage Reports:** Approve, reject, or mark reports as completed.
- **Create/Edit Events:** Add new events, edit existing ones, and categorize.
- **Create/Edit Announcements:** Publish important municipal news.
- **User Management:** (If implemented) View and manage user accounts.

### Example Admin Workflow
1. Log in to the admin dashboard.
2. Review new service requests and assign priorities.
3. Approve or reject citizen reports.
4. Create a new event or announcement.
5. Monitor system analytics and user engagement.

## Troubleshooting & FAQ
- **Database not found?** Ensure `Data/municipal.db` exists or run `dotnet ef database update`.
- **Login issues?** Confirm credentials and check if the database is seeded.
- **Session timeout?** Log in again; sessions expire after 30 minutes of inactivity.
- **How to reset admin password?** Update the password in the database or reseed.
- **How to add more sample data?** Edit `Data/SeedData.cs` and restart the app.

## Admin Login
- admin@example.com
- Admin@123
