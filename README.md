# PRN222-Final Project Overview

This project is a multi-interface web application using ASP.NET Core, featuring both MVC and Razor Pages in a single solution:

- **Run the MVC project** to start the application. Razor Pages will be available automatically alongside MVC.
- **MVC** is designed for customers and guests, accessible via URLs with the `/mvc/` prefix (e.g., `/mvc/auth/login`).
- **Razor Pages** are for admin users, accessible via URLs with the `/razor/` prefix (e.g., `/razor/Index`).
- On the login page, if you log in as an admin, you will be redirected to the Razor admin interface. Customers and guests will be directed to the MVC interface.

---

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

### 4. Create SharedImages Folder
**Important**: Create a `SharedImages` folder at the root level of the project (same level as BLL, DAL, MVC, Razor folders). This folder is required for storing uploaded product images.

Project structure should look like:
```
PRN222-Final/
├── BLL/
├── DAL/
├── MVC/
├── Razor/
├── SharedImages/  ← Create this folder
└── README.md
```

### 5. Verify Setup
1. Run the application
2. If you encounter any database connection errors, double-check:
   - Database name is correct
   - Server name is correct
   - SQL Server is running
   - Username and password are correct
   - Connection string format is correct
   - SharedImages folder exists at the root level

### 6. Project Structure
The project uses a three-layer architecture:
- **DAL (Data Access Layer)**: Contains database models and context
- **BLL (Business Logic Layer)**: Contains business logic and services
- **MVC & Razor**: Presentation layers for different user interfaces

The `DemoContext.cs` in the DAL project handles all database connections and entity relationships. Make sure the connection string in `appsettings.json` matches your SQL Server configuration for the application to work correctly.

---

## SignalR Configuration Guide

### Overview
This project uses SignalR for real-time notifications between the MVC (client) and Razor (admin) applications. When a customer places an order in MVC, it triggers a real-time notification to the admin interface in Razor.

### Architecture
- **SignalR Hub Host**: Razor (Admin) application
- **SignalR Client**: MVC (Customer) application
- **Real-time Flow**: Customer order → MVC → SignalR Hub → Admin notification

### Configuration Steps

#### 1. Update Port Numbers
**Each team member must use different ports to avoid conflicts.**

**For Razor (Admin) app:**
1. Open `Razor/Properties/launchSettings.json`
2e the port numbers:
```json
[object Object]profiles":[object Object]  Razor: {
    commandName": Project",
      dotnetRunMessages: true,
    launchBrowser": true,
     applicationUrl": https://localhost:7082;http://localhost:5082",
environmentVariables: [object Object]   ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

**For MVC (Client) app:**
1. Open `MVC/Properties/launchSettings.json`
2e the port numbers:
```json
[object Object]profiles: {  MVC: {
    commandName": Project",
      dotnetRunMessages: true,
    launchBrowser": true,
     applicationUrl": https://localhost:7086;http://localhost:5086",
environmentVariables: [object Object]   ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

####2SignalR Connection URLs
After changing ports, update all SignalR connection URLs in your code:

**In `BLL/Service/OrderService.cs`:**
```csharp
_connection = new HubConnectionBuilder()
    .WithUrl(https://localhost:7082/DataSignalRChanel") // Update to your Razor port
    .WithAutomaticReconnect()
    .Build();
```

**In `MVC/wwwroot/js/site.js`:**
```javascript
var connection = new signalR.HubConnectionBuilder()
    .withUrl(https://localhost:7082/DataSignalRChanel") // Update to your Razor port
    .build();
```

#### 3. Update CORS Configuration
**In `Razor/appsettings.json`:**
```json
{
 AllowedOrigins: [https://localhost:7086 // Update to your MVC port
  ]
}
```

####4Verify SignalR Hub Mapping
Ensure the SignalR hub is properly mapped in `Razor/Program.cs`:
```csharp
app.MapHub<DataSignalR>("/DataSignalRChanel");
```

### How It Works

1. **Customer places order** in MVC application
2. **MVC OrderService** connects to Razor's SignalR hub
3*SignalR hub** notifies all connected admin browsers
4. **Admin OrderPage** receives notification and reloads automatically

### Troubleshooting

#### Common Issues:

**1. Port Conflicts**
- **Error**: "Address already in use"
- **Solution**: Change ports in `launchSettings.json` for both projects

**2CORS Errors**
- **Error**: "Failed to complete negotiation with the server"
- **Solution**: Update `AllowedOrigins` in `Razor/appsettings.json` to match your MVC port

**3. Connection Failed**
- **Error**: "Failed to start the connection"
- **Solution**: 
  - Ensure Razor app is running before MVC
  - Check that SignalR hub URL matches your Razor port
  - Verify hub mapping in `Razor/Program.cs`

**4. Duplicate Hub Mapping**
- **Error**: "AmbiguousMatchException: The request matched multiple endpoints"
- **Solution**: Remove duplicate `app.MapHub<DataSignalR>("/DataSignalRChanel");` lines in `Razor/Program.cs`

### Team Member Setup Checklist

For each team member, ensure:

- [ ] Different ports assigned to MVC and Razor apps
- [ ] SignalR connection URLs updated in `OrderService.cs` and `site.js`
- [ ] CORS configuration updated in `Razor/appsettings.json`
- [ ] No duplicate hub mappings in `Razor/Program.cs`
- [ ] Razor app starts before MVC app
- Both apps can run simultaneously without conflicts

### Testing SignalR1 **Start Razor app** (admin interface)
2 **Start MVC app** (customer interface)
3. **Login as admin** in Razor app and navigate to OrderPage
4. **Login as customer** in MVC app and place an order
5*Verify** that the admin OrderPage automatically reloads when the order is placed

### File Locations Summary

| File | Purpose | Update Required |
|------|---------|-----------------|
| `Razor/Properties/launchSettings.json` | Razor app port | Yes |
| `MVC/Properties/launchSettings.json` | MVC app port | Yes |
| `BLL/Service/OrderService.cs` | SignalR client connection | Yes |
| `MVC/wwwroot/js/site.js` | Browser SignalR connection | Yes |
| `Razor/appsettings.json` | CORS configuration | Yes |
| `Razor/Program.cs` | Hub mapping | No (unless duplicate) |
