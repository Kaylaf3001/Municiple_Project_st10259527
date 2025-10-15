# Municipal Report Management System

A comprehensive web application for managing municipal reports with user and administrator interfaces. This system allows citizens to submit reports about municipal issues and enables administrators to review, manage, and track the status of these reports.

## Table of Contents
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Installation](#installation)
- [Database Schema](#database-schema)
- [Project Structure](#project-structure)
- [Data Structures](#data-structures)
- [Services](#services)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
- [Screenshots](#screenshots)
- [Admin Login](#admin-login)

## Features

## Features

### 🧍 User Features
- Register and log in securely
- Submit and manage municipal reports
- View personal report history
- Track report statuses (Pending, In Review, Completed, etc.)
- View and filter **events** by date, category, or keyword
- View all **announcements**
- Get personalized **recommended events** based on search history
- Update personal profile information

### 🧑‍💼 Admin Features
- Dashboard with visual report statistics
- Manage and filter reports by type, status, and user
- Create, edit, and delete **events** and **announcements**
- Manage user accounts and roles
- Export report or event data for analysis
- View all submitted reports and activities through searchable tables
  
## Technology Stack

### Backend
- **Framework**: ASP.NET Core
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity

### Frontend
- **UI Framework**: Bootstrap 5
- **Client-side Scripting**: JavaScript, jQuery

## Data Structure
---

### HashSet
<img width="1004" height="534" alt="image" src="https://github.com/user-attachments/assets/e2e4a359-c5a1-49d5-ae7d-b576d2dca8ea" />
<img width="771" height="604" alt="image" src="https://github.com/user-attachments/assets/0d3f0045-bea3-4325-b637-95b10b9cd0b4" />

### Dictionary
<img width="1035" height="541" alt="image" src="https://github.com/user-attachments/assets/f4f4308b-06ab-4e73-8af8-a7bba128502f" />

### Database
<img width="1262" height="1224" alt="image" src="https://github.com/user-attachments/assets/4707624b-13d9-4f88-95d7-58f9903fc10e" />

### IQueryable
<img width="923" height="836" alt="image" src="https://github.com/user-attachments/assets/13080132-2050-4b36-93a0-8d47dfbe4f94" />

### IEnumerable
<img width="446" height="120" alt="image" src="https://github.com/user-attachments/assets/12be66f9-8bf5-4117-a4fe-00328afd52a2" />
<img width="1501" height="634" alt="image" src="https://github.com/user-attachments/assets/74a74470-82c8-4c08-9197-eda48b2a6047" />

### Stacks and Dictionary
<img width="726" height="630" alt="image" src="https://github.com/user-attachments/assets/1e4ff19d-f483-40ea-9aca-f5505b3cfecc" />

### Queues
<img width="752" height="751" alt="image" src="https://github.com/user-attachments/assets/9bc48010-bf7d-4aee-9f24-c5387b904034" />

### Stacks
<img width="792" height="769" alt="image" src="https://github.com/user-attachments/assets/799c85a3-3112-4544-beb2-d85b8da874ee" />

## Services

### 🗓️ EventAnnouncementService
A centralized service managing event and announcement data using **dictionaries and hashsets** for fast filtering.

Key functionalities:
- Initialize indexes for events and announcements  
- Filter events by:
  - Date range  
  - Category  
  - Search term  
- Retrieve announcements by date  
- Retrieve available event categories  

### Recommendation Service
The RecommendationService class implements a stack- and queue-based recommendation algorithm that dynamically suggests upcoming municipal events to users based on their recent search history.

## How It Works
1. **Search History Stack:**
- The algorithm retrieves the user's latest searches from the database and stores them in a Stack<UserSearchHistory>.
- The stack ensures the most recent searches are processed first, reflecting current user interests.

2. **Upcoming Events Queue:**
- Upcoming events are retrieved from the repository as a Queue<EventModel>.
- This queue represents events in chronological order.

3. **Scoring and Matching:**
- Each event is scored against search terms and categories from the user’s history.
- Matches increase an event’s score depending on relevance (e.g., category match = higher score).

4. **Dictionary for Event Ranking:**
- A Dictionary<int, (EventModel Event, int Score)> stores each event with its cumulative score.
- The dictionary allows constant-time lookups and efficient score updates.

5. **Final Recommendation Stack:**
- Events are sorted by score and date, then pushed into a Stack<EventModel>.
- This stack represents the final set of top recommended events (limited to 5).

**Display:** 
<img width="1265" height="309" alt="image" src="https://github.com/user-attachments/assets/533ba3ff-7e27-403a-822c-53869a5dc66f" />


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
- FirstName
- LastName
- Email
- Password
- IsAdmin

### Reports Table
- ReportId (PK)
- UserId (FK)
- ReportType
- Description
- ReportDate
- Location
- Status (Pending/InReview/Approved/Rejected/Completed)
- UpdatedAt

### Events Table
- EventId
- Title
- Location
- Date
- Category
- Description
- Status
- UserId

### Announcements Table
- AnnouncementId
- Title
- Date
- Description
- Location
- Status
- UserId

### UserSearchHistory
- SearchId
- UserId
- SearchTerm
- Category
- SearchDate


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
5. **Events & Announcements**: Create and manage events and announcements with system analytics and tracking

## API Endpoints

### User Endpoints
- `GET /User/Register` - Display registration form
- `POST /User/Register` - Create new user account
- `GET /User/Login` - Display login form
- `POST /User/Login` - Authenticate user
- `GET /User/Profile` - View user profile
- `POST /User/Profile` - Update user profile
- `GET /User/History` - View user activity history

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

### Events & Announcements Endpoints
- `GET /Events` - List upcoming events (User)
- `GET /Events/{id}` - View event details
- `POST /Events/Create` - Create new event (Admin)
- `GET /Announcements` - View all announcements
- `OST /Announcements/Create` - Create announcement (Admin)

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
<img width="1253" height="425" alt="image" src="https://github.com/user-attachments/assets/d345ffed-a7ed-4b9d-ba8e-10bf6f560a52" />

4. ** Local Events and Announcements Dashboard**
<img width="1875" height="858" alt="image" src="https://github.com/user-attachments/assets/ef3c7d53-afa1-4eb7-b6f6-c99d2ad8bb65" />

### Admin Interface
5. **Admin Dashboard**
<img width="2497" height="1222" alt="image" src="https://github.com/user-attachments/assets/426144a5-651c-4ac9-8fbb-93fb2adf7ddc" />

6. **Report Management**
<img width="2496" height="1131" alt="image" src="https://github.com/user-attachments/assets/c9f6dcba-d7dd-4fb4-8096-a3b8da5da097" />
<img width="2514" height="1130" alt="image" src="https://github.com/user-attachments/assets/1c82cf7b-5355-49fc-b0c9-287c81220886" />

7. **Add Events**
<img width="1861" height="862" alt="image" src="https://github.com/user-attachments/assets/ea901321-653c-4efb-8e81-78408228bde8" />

8. **Manage Events**
<img width="1860" height="918" alt="image" src="https://github.com/user-attachments/assets/5024690b-c3a1-4840-9526-0d92dfea4b6b" />

9. Add Announcements
<img width="1857" height="918" alt="image" src="https://github.com/user-attachments/assets/e4f79a3c-270a-4747-8c35-a15cf3fafcee" />

10. Manage Announcements
<img width="1858" height="919" alt="image" src="https://github.com/user-attachments/assets/51b88471-af6e-4112-a7d6-54c33ff49b6b" />

## Admin Login
- admin@example.com
- Admin@123
