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
1. Open `MVC/appsettings.json`; if `appssettings.json` don't exists, create it
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
    "DefaultConnection": "Server=LAPTOP-F90M82DI\\MSSQLSEVER; Database=PRN222-Final;User ID=sa;Password=123456;Trust Server Certificate=True"
  }
}
```

3. Open `Razor/appsettings.json`; if `appssettings.json` don't exists, create it
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
    "DefaultConnection": "Server=LAPTOP-F90M82DI\\MSSQLSEVER; Database=PRN222-Final;User ID=sa;Password=123456;Trust Server Certificate=True"
  }
}
```

Replace `YOUR_SERVER_NAME` with your SQL Server instance name (e.g., `localhost` or `.\SQLEXPRESS`)

### 4. Verify Setup
1. Run the application
2. If you encounter any database connection errors, double-check:
   - Database name is correct
   - Server name is correct
   - SQL Server is running
   - Connection string format is correct
