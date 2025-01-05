# Bank System

## Overview
The **Bank System** is a Windows Forms application designed to manage banking operations. This project demonstrates the use of a layered architecture and integrates with a database for backend operations.

## Features
- User-friendly GUI developed with Windows Forms.
- Integration with a database for managing banking data.
- Key functionalities:
  - Adding and managing customer data.
  - Handling transactions.
  - Generating reports.

## Database
- **ERD File:** The `Bank System ERD.pdf` provides the Entity-Relationship Diagram for the database schema.
- **SQL Scripts:**
  - `Data to Populate Database.sql`: Script for populating the database with initial data.
  - `Generated SQL.sql`: Script for creating the database schema.

## Requirements
- **Software:**
  - Visual Studio (tested with version 2022 or later).
  - .NET Framework 4.7.2.
- **Database:**
  - SQL Server (Express Edition or higher).

## Getting Started
1. Clone the repository and open the `Bank System.sln` file in Visual Studio.
2. Configure the connection string in `App.config` to point to your SQL Server database.
3. Run the SQL scripts (`Generated SQL.sql` and `Data to Populate Database.sql`) to set up the database.
4. Build and run the solution in Visual Studio.

## Development Notes
- The main application logic is located in `Form1.cs`.
- All resources and settings are managed under the `Properties/` folder.
- Ensure the database is set up and accessible before running the application.

## Future Enhancements
- Add more features like account types, loan processing, and transaction history.
- Enhance the UI for better user experience.
- Implement role-based access control for administrators and users.
