# PawsAndClaws 🐾

## Description

- Web application for browsing and applying to adopt pets.
- Core features: pet listings, adoption applications, user accounts, admin management.

## Project Architecture

| Component | Technology |
|---|---|
| Programming Language | C# |
| IDE | Visual Studio |
| Backend | ASP.NET Core MVC (.NET 10) |
| Data | Entity Framework Core |
| Database Management System | SQL Server (containerized via Docker Compose) |
| UI | Razor views, Tailwind CSS, vendor libraries in `wwwroot` |

## Project Structure

- `PawsAndClaws/` — main project
- `Controllers/` — MVC controllers
- `Data/` — DbContext and seed data
- `Migrations/` — EF Core migrations
- `Models/` — domain and view models
- `Views/` — Razor views and partials
- `wwwroot/` — static assets (css, js, lib, images)
- `docker-compose.yml` — local SQL Server service
- `.env.example` — environment template (do NOT commit secrets)

## How to Run

1. Install .NET 10 SDK and Docker.
2. Copy `.env.example` to `.env` and fill values locally.
3. Start the database:

```bash
docker-compose up -d
```

4. Run the app from repository root:

```bash
dotnet run --project PawsAndClaws
```

Open the app at the URL shown in the console.

## Group Members

| Name | Role |
|---|---|
| Argallon, Dazel C. | Backend Development Lead |
| Castillo, Julianna Leila T. | UI/UX & Frontend Lead |
| Gragas, Nethan Edry L. | Database Architecture Lead |
| Jocson, Dan Louie M. | Technical Documentation Specialist |

## Affiliation

Polytechnic University of the Philippines — Bachelor of Science in Computer Science 3-2, Group 14

## Course Context

Made for Applications Development and Emerging Technologies
