-- Create Database
CREATE DATABASE ElectricGadgetDB;
GO

USE ElectricGadgetDB;
GO

-- Create Users Table
CREATE TABLE Users (
    UserID VARCHAR(50) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    IsLocked BIT DEFAULT 0,
    FailedAttempts INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Insert a default Super Admin user (Password is 'admin123' hashed with SHA256)
-- Hash for 'admin123' is 240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9
INSERT INTO Users (UserID, Name, Email, PasswordHash, Role) 
VALUES ('superadmin', 'Super Admin', 'admin@electricgadget.com', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Super Admin');

-- Create Branches Table
CREATE TABLE Branches (
    BranchID INT IDENTITY(1,1) PRIMARY KEY,
    BranchName NVARCHAR(100) NOT NULL,
    Location NVARCHAR(200) NOT NULL
);

-- Create Products Table
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    Stock INT NOT NULL,
    Description NVARCHAR(MAX),
    BranchID INT FOREIGN KEY REFERENCES Branches(BranchID)
);

-- Create Orders Table
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    UserID VARCHAR(50) FOREIGN KEY REFERENCES Users(UserID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE()
);

-- Create ServiceRequests Table
CREATE TABLE ServiceRequests (
    RequestID INT IDENTITY(1,1) PRIMARY KEY,
    UserID VARCHAR(50) FOREIGN KEY REFERENCES Users(UserID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    IssueDescription NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    RequestDate DATETIME DEFAULT GETDATE()
);

-- Create Inventory Table
CREATE TABLE Inventory (
    InventoryID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    StockIn INT DEFAULT 0,
    StockOut INT DEFAULT 0,
    UpdatedAt DATETIME DEFAULT GETDATE()
);

-- Create Reviews Table
CREATE TABLE Reviews (
    ReviewID INT IDENTITY(1,1) PRIMARY KEY,
    UserID VARCHAR(50) FOREIGN KEY REFERENCES Users(UserID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(MAX),
    ReviewDate DATETIME DEFAULT GETDATE()
);

-- Create Notifications Table
CREATE TABLE Notifications (
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID VARCHAR(50) FOREIGN KEY REFERENCES Users(UserID),
    Message NVARCHAR(MAX) NOT NULL,
    IsRead BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Create LoginLogs Table
CREATE TABLE LoginLogs (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    UserID VARCHAR(50) FOREIGN KEY REFERENCES Users(UserID),
    AttemptTime DATETIME DEFAULT GETDATE(),
    IsSuccess BIT NOT NULL
);

-- Create Commission Table
CREATE TABLE Commission (
    CommissionID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    Percentage DECIMAL(5, 2) NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL
);

-- Create Wishlist Table
CREATE TABLE Wishlist (
    WishlistID INT IDENTITY(1,1) PRIMARY KEY,
    UserID VARCHAR(50) FOREIGN KEY REFERENCES Users(UserID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    ReminderDate DATETIME
);
GO
