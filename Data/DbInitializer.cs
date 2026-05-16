using System.Security.Cryptography;
using System.Text;
using ElectricGadget.Web.Models.Entities;

namespace ElectricGadget.Web.Data
{
    public static class DbInitializer
    {
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public static void Initialize(ApplicationDbContext context)
        {
            // Safe column additions via raw SQL to avoid migration conflicts
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(
                "Server=(localdb)\\mssqllocaldb;Database=ElectricGadgetStoreDb;Trusted_Connection=True;TrustServerCertificate=True;"))
            {
                connection.Open();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    IF COL_LENGTH('Products','CategoryId') IS NULL
                        ALTER TABLE Products ADD CategoryId INT NULL;
                    IF COL_LENGTH('Products','BrandId') IS NULL
                        ALTER TABLE Products ADD BrandId INT NULL;
                    IF COL_LENGTH('Products','DiscountPrice') IS NULL
                        ALTER TABLE Products ADD DiscountPrice DECIMAL(18,2) NULL;
                    IF COL_LENGTH('Products','Model') IS NULL
                        ALTER TABLE Products ADD Model NVARCHAR(MAX) NULL;
                    IF COL_LENGTH('Products','Warranty') IS NULL
                        ALTER TABLE Products ADD Warranty NVARCHAR(MAX) NULL;
                    IF COL_LENGTH('Products','IsPublished') IS NULL
                        ALTER TABLE Products ADD IsPublished BIT NOT NULL DEFAULT 1;
                    IF COL_LENGTH('Users','IsActive') IS NULL
                        ALTER TABLE Users ADD IsActive BIT NOT NULL DEFAULT 1;
                    IF COL_LENGTH('Users','BranchId') IS NULL
                        ALTER TABLE Users ADD BranchId INT NULL;
                    IF COL_LENGTH('Users','CanAccessInventory') IS NULL
                        ALTER TABLE Users ADD CanAccessInventory BIT NOT NULL DEFAULT 1;
                    IF COL_LENGTH('Users','CanAccessBilling') IS NULL
                        ALTER TABLE Users ADD CanAccessBilling BIT NOT NULL DEFAULT 1;
                    IF COL_LENGTH('Users','CanDownloadReports') IS NULL
                        ALTER TABLE Users ADD CanDownloadReports BIT NOT NULL DEFAULT 0;
                    IF COL_LENGTH('Users','CanManageUsers') IS NULL
                        ALTER TABLE Users ADD CanManageUsers BIT NOT NULL DEFAULT 0;

                    -- Create missing tables if they don't exist
                    IF OBJECT_ID('Branches', 'U') IS NULL
                    CREATE TABLE Branches (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name NVARCHAR(MAX) NOT NULL,
                        Location NVARCHAR(MAX) NULL,
                        ManagerId NVARCHAR(450) NULL,
                        IsActive BIT NOT NULL DEFAULT 1,
                        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                    );

                    IF OBJECT_ID('AuditLogs', 'U') IS NULL
                    CREATE TABLE AuditLogs (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        UserId NVARCHAR(MAX) NOT NULL,
                        UserName NVARCHAR(MAX) NOT NULL,
                        Action NVARCHAR(MAX) NOT NULL,
                        Module NVARCHAR(MAX) NOT NULL,
                        Details NVARCHAR(MAX) NOT NULL,
                        Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
                        IpAddress NVARCHAR(MAX) NULL
                    );

                    IF OBJECT_ID('SystemSettings', 'U') IS NULL
                    CREATE TABLE SystemSettings (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        CompanyName NVARCHAR(MAX) NOT NULL,
                        Currency NVARCHAR(MAX) NOT NULL,
                        Timezone NVARCHAR(MAX) NOT NULL,
                        Language NVARCHAR(MAX) NOT NULL,
                        LogoUrl NVARCHAR(MAX) NULL,
                        SupportEmail NVARCHAR(MAX) NULL,
                        Phone NVARCHAR(MAX) NULL,
                        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
                    );
                ";
                cmd.ExecuteNonQuery();
            }

            context.Database.EnsureCreated();

            // ── Categories ──────────────────────────────────────────────────────────
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Smartphone" },
                    new Category { Name = "Laptop" },
                    new Category { Name = "Smart TV" },
                    new Category { Name = "Headphones" },
                    new Category { Name = "Speaker" },
                    new Category { Name = "Smart Watch" }
                );
                context.SaveChanges();
            }

            // ── Default Branch ──────────────────────────────────────────────────────
            if (!context.Branches.Any())
            {
                context.Branches.Add(new Branch
                {
                    Name = "Main Branch",
                    Location = "Dhaka, Bangladesh",
                    IsActive = true
                });
                context.Branches.Add(new Branch
                {
                    Name = "Chittagong Branch",
                    Location = "Chittagong, Bangladesh",
                    IsActive = true
                });
                context.SaveChanges();
            }

            // ── Super Admin ─────────────────────────────────────────────────────────
            var superAdmin = context.Users.FirstOrDefault(u => u.UserID == "superadmin");
            if (superAdmin == null)
            {
                context.Users.Add(new User
                {
                    UserID          = "superadmin",
                    Name            = "Super Administrator",
                    Password        = "superadmin123",
                    PasswordHash    = HashPassword("superadmin123"),
                    Role            = "Super Admin",
                    Email           = "superadmin@electric.com",
                    IsActive        = true,
                    IsLocked        = false,
                    FailedAttempts  = 0,
                    CanAccessInventory   = true,
                    CanAccessBilling     = true,
                    CanDownloadReports   = true,
                    CanManageUsers       = true
                });
                context.SaveChanges();
            }
            else
            {
                // Force unlock and reset superadmin on each app start for convenience
                superAdmin.IsLocked = false;
                superAdmin.FailedAttempts = 0;
                superAdmin.IsActive = true;
                // Also ensure password is reset to default if needed, or just leave it
                context.SaveChanges();
            }

            // ── Admin ───────────────────────────────────────────────────────────────
            var adminUser = context.Users.FirstOrDefault(u => u.UserID == "admin");
            if (adminUser == null)
            {
                context.Users.Add(new User
                {
                    UserID          = "admin",
                    Name            = "Shop Manager",
                    Password        = "admin",
                    PasswordHash    = HashPassword("admin"),
                    Role            = "Admin",
                    Email           = "manager@electric.com",
                    IsActive        = true,
                    IsLocked        = false,
                    FailedAttempts  = 0,
                    CanAccessInventory = true,
                    CanAccessBilling   = true
                });
                context.SaveChanges();
            }
            else
            {
                adminUser.IsLocked = false;
                adminUser.FailedAttempts = 0;
                adminUser.IsActive = true;
                context.SaveChanges();
            }

            // ── System Settings ─────────────────────────────────────────────────────
            if (!context.SystemSettings.Any())
            {
                context.SystemSettings.Add(new SystemSettings
                {
                    CompanyName  = "Electric Gadget Store",
                    Currency     = "BDT",
                    Timezone     = "Asia/Dhaka",
                    Language     = "English",
                    SupportEmail = "support@electric.com"
                });
                context.SaveChanges();
            }

            // ── Brands ──────────────────────────────────────────────────────────────
            if (!context.Brands.Any())
            {
                var smartphone = context.Categories.FirstOrDefault(c => c.Name == "Smartphone");
                var laptop     = context.Categories.FirstOrDefault(c => c.Name == "Laptop");
                var tv         = context.Categories.FirstOrDefault(c => c.Name == "Smart TV");

                context.Brands.AddRange(
                    new Brand { Name = "Apple",   CategoryId = smartphone?.Id ?? 1 },
                    new Brand { Name = "Samsung",  CategoryId = smartphone?.Id ?? 1 },
                    new Brand { Name = "HP",       CategoryId = laptop?.Id ?? 2 },
                    new Brand { Name = "Sony",     CategoryId = tv?.Id ?? 3 }
                );
                context.SaveChanges();
            }
        }
    }
}
