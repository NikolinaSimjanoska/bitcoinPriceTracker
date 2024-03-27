# Bitcoin Price Tracker Application

This application is designed to retrieve and store the current price of Bitcoin automatically and provide endpoints for accessing this data. It utilizes .NET framework and SQLite as the database.

## Features

- **Automatic Price Retrieval:** The application fetches the current price of Bitcoin hourly and stores it in the database.
- **API Endpoints:**
  - `GetCurrentPrice`: Returns the current price of Bitcoin.
  - `GetAveragePriceForDate`: Retrieves all records for a specific date from the database and calculates the average price.
- **Seeder Functionality:** Seeds random hourly Bitcoin prices for the last 14 days if the database is empty.
- **Swagger Integration:** Endpoints are accessible through Swagger for easy testing and documentation.

## Technologies Used

- **.NET:** https://dotnet.microsoft.com/
- **SQLite:** https://www.sqlite.org/index.html
- **Swagger:** https://swagger.io/
