# FU News Management (PRN232 Assignment 01)

## Setup
1. Create the SQL Server database `FUNewsManagement` (script from the course).
2. In `21_NguyenQuangHuy_Assignment01_BackEnd/`, copy `appsettings.example.json` to `appsettings.json`
   and fill in:
   - `AdminAccount:Password` (admin email: `admin@FUNewsManagementSystem.org`)
   - `Jwt:Key` (random string, at least 32 characters)
   - `ConnectionStrings:DefaultConnection` if your SQL Server differs.
3. Run BackEnd (http://localhost:5001) then FrontEnd (http://localhost:5002):
   `dotnet run --urls http://localhost:5001` / `dotnet run --urls http://localhost:5002`
