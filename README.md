# WorldCities

## Description
A full stack ASP.NET and Angular application that displays country and city data for the user.

## Database
This applicaiton uses MySQL database. When installing this application, you will need to set up a connection to your database using the .NET secrets feature.

To set up the database connection, follow these steps:

1. Initialize user secrets:
   ```bash
   dotnet user-secrets init
   ```
2. Set your database string:
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=your-server;Database=your-database;User=your-username;Password=your-password;"
   ```
