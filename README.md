# PRN222-Final Project Setup Guide

## Database Setup Instructions

### 1. Create Database
1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Create a new database named `PRN222-Final`

### 2. Initialize Database
1. Open the `db.txt` file in the project
2. Copy all the SQL code from the file
3. In SSMS, select the `PRN222-Final` database
4. Paste and execute the copied SQL code to create all necessary tables and initial data

### 3. Update Connection Strings
1. Open `MVC/appsettings.json`; if `appsettings.json` doesn't exist, create it
2. Update the `DefaultConnection` string with your SQL Server details:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=PRN222-Final;User ID=YOUR_USERNAME;Password=YOUR_PASSWORD;Trust Server Certificate=True"
  }
}
```

3. Open `Razor/appsettings.json`; if `appsettings.json` doesn't exist, create it
4. Update the `DefaultConnection` string with the same connection string:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=PRN222-Final;User ID=YOUR_USERNAME;Password=YOUR_PASSWORD;Trust Server Certificate=True"
  }
}
```

Replace:
- `YOUR_SERVER_NAME` with your SQL Server instance name (e.g., `localhost` or `.\SQLEXPRESS`)
- `YOUR_USERNAME` with your SQL Server username
- `YOUR_PASSWORD` with your SQL Server password

### 4. Verify Setup
1. Run the application
2. If you encounter any database connection errors, double-check:
   - Database name is correct
   - Server name is correct
   - SQL Server is running
   - Username and password are correct
   - Connection string format is correct

### 5. Project Structure
The project uses a three-layer architecture:
- **DAL (Data Access Layer)**: Contains database models and context
- **BLL (Business Logic Layer)**: Contains business logic and services
- **MVC & Razor**: Presentation layers for different user interfaces

The `DemoContext.cs` in the DAL project handles all database connections and entity relationships. Make sure the connection string in `appsettings.json` matches your SQL Server configuration for the application to work correctly.
