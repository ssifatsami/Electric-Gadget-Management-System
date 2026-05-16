
# Electric Gadget Management System

A comprehensive hybrid management solution built using ASP.NET Core MVC for web access and C# WinForms for a robust desktop experience. It facilitates multi-branch inventory tracking, customer shopping, and advanced administrative control for electronic retail businesses.


## Acknowledgements

 - [Awesome Readme Templates](https://awesomeopensource.com/project/elangosundar/awesome-README-templates)
 - [Awesome README](https://github.com/matiassingers/awesome-readme)
 - [How to write a Good readme](https://bulldogjob.com/news/449-how-to-write-a-good-readme-for-your-github-project)


## Features

Role-Based Access Control: Dedicated dashboards for Super Admins, Branch Admins, and Customers.
Hybrid Synchronization: Unified SQL Server database ensuring real-time data consistency between web and desktop interfaces.
Advanced Product Comparison: Side-by-side feature and price comparison tool implemented in both Web and WinForms.
Multi-Branch Management: Tools for Super Admins to monitor sales, manage branches, and assign branch-specific managers.
Data Recovery Center: Integrated manual and automated system backup tools with local file export capabilities.
Currency Localization: Full support for Bangladeshi Taka (৳) with localized pricing and billing.


## API Reference

GET /Product/Index: Browse all products with dynamic filtering (Price, Brand, Rating).
POST /Product/Compare: API endpoint to fetch comparison data for selected product IDs.
GET /Product/TopSelling: Retrieve the top 10 most popular products based on sales data.
POST /SuperAdmin/CreateBackup: Administrative endpoint for triggering system data exports.
## Environment Variables

To run this project, you will need to add the following to your appsettings.json:
DefaultConnection: Your SQL Server connection string.
ASPNETCORE_URLS: http://localhost:5255


## Installation

1.Ensure .NET 6+ SDK and SQL Server are installed on your machine.
2.Clone the repository and navigate to the project folder.
3.Restore dependencies: dotnet restore
4.Apply database migrations: dotnet ef database update
5.Launch the Web Server: dotnet run
6.Launch the Desktop App: Open the solution in Visual Studio and run the Electric_Gadget_Management project.
```
    
## Authors

1. MUHAMMAD SIFAT AL SAMI — 24-60084-3
2. MOUMITA RUBAYET AUROMA — 25-60724-1
3. TASMIN SAFA — 24-60248-3
4. TASFIA TAHIAT — 24-57529-2

"Object-Oriented Programming 2",Section EE
American International University-Bangladesh

## Deployment

Web: Ready for deployment on IIS (Internet Information Services) or cloud platforms like Azure.
Desktop: Can be published as a standalone .exe using ClickOnce or Folder distribution for Windows clients.

## Color Reference

The UI follows a modern SaaS-inspired professional color palette:

Primary Blue: #3b82f6 (Used for primary actions and navigation)
Success Green: #10b981 (Used for payments and system restores)
Danger Red: #ef4444 (Used for deletions and critical warnings)
Admin Sidebar: #1e293b (A sleek dark navy for administrative interfaces)

## FAQ

## Can the desktop app work without internet?

 Yes, the WinForms dashboard can operate within a local network as long as it has access to the SQL Server.

## How do I recover data if the system fails?

Use the "Security Recovery" panel in the Super Admin dashboard to restore data from a previously saved .bak file.


## Tech Stack

Frontend: Razor Views (Web), Windows Forms (Desktop), Bootstrap 5, Vanilla CSS/JS.
Backend: .NET 6.0 / C#.
Database: Microsoft SQL Server with Entity Framework Core (Code First).
Security: Session-based authentication and role-specific permissions.


## Documentation

The technical documentation for the Electric Gadget Management System covers the following areas:

Architecture: A detailed guide on the 3-Tier Layered Architecture (Presentation, Business Logic, and Data Access Layers).
Database Schema: Complete ER Diagram documentation showing the relationships between Products, Brands, Categories, Branches, and Users.
Admin Guide: Step-by-step instructions for Super Admins on how to manage branch-level permissions and system recovery protocols.
User Manual: A guide for customers on how to browse, filter, compare, and purchase gadgets through the web portal.
Developer Guide: Instructions on how to extend the entity models and implement new branch-specific logic using EF Core.

## Roadmap

Phase 1 (Completed): Core Web MVC and WinForms integration, multi-branch support, and basic inventory CRUD.
Phase 2 (Completed): Implementation of the side-by-side comparison engine and BDT currency localization.
Phase 3 (Current): System stabilization, audit logging enhancement, and local data export optimizations.
Phase 4 (Planned): Integration of an AI-based recommendation system to suggest gadgets based on user comparison history.
Phase 5 (Future): Mobile app development using .NET MAUI to provide real-time stock alerts to branch managers.


## Used By

Small to Medium Electronic Retailers: Stores that need to manage inventory across multiple physical locations while maintaining a web presence.
Gadget Resellers: Independent businesses focusing on smartphones, laptops, and peripheral gadgets.
Academic Institutions: As a reference implementation for Hybrid Web/Desktop .NET applications and Layered Architecture design.


## Lessons Learned
Developing this hybrid system provided several key technical insights:

STAThread Management: Discovered the critical importance of [STAThread] attributes when launching WinForms common dialogs (like SaveFileDialog) from a combined Web/Desktop entry point.
Thread Safety in Hybrid Apps: Learned how to effectively offload synchronous IO operations to background tasks to prevent UI hangs while the web server is running in parallel.
Database Synchronization: Mastered the use of Entity Framework Core to maintain data integrity across two different presentation layers (Web and Desktop) simultaneously.
Localization Strategy: Implementing a project-wide currency shift (USD to BDT) highlighted the benefits of centralized UI templates and string formatting helpers.


## Screenshots

https://drive.google.com/drive/folders/1Jgsf9_lqlJe3n6hoOwhJm_r7yVaoTjAY?usp=sharing


## 🔗 Links
[![portfolio](https://img.shields.io/badge/my_portfolio-000?style=for-the-badge&logo=ko-fi&logoColor=white)](https://katherineoelsner.com/)
[![linkedin](https://img.shields.io/badge/linkedin-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/)
[![twitter](https://img.shields.io/badge/twitter-1DA1F2?style=for-the-badge&logo=twitter&logoColor=white)](https://twitter.com/)


## 🚀 About Me
👋 Hi, I'm Sifat 
🎓 Studing at AIUB Department of Data Science
💻 Beginner Web Developer from Bangladesh  
🌱 Currently learning HTML, CSS & JavaScript  
🚀 Building creative and romantic web projects  
🎯 Goal: Become a professional full-stack developer

