using System.ComponentModel.DataAnnotations;

namespace ElectricGadget.Web.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();
    }

    public class Brand
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }

    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public int Stock { get; set; }
        public string? ImageUrl { get; set; } = "/images/default.jpg";
        public int BrandId { get; set; }
        public virtual Brand Brand { get; set; } = null!;
        public int? CategoryId { get; set; } // Nullable temporarily for existing records
        public virtual Category Category { get; set; } = null!;
        public string? Model { get; set; }
        public string? Warranty { get; set; }
        public bool IsPublished { get; set; } = true;
        public decimal? DiscountPrice { get; set; }
        public virtual ICollection<Feature> Features { get; set; } = new List<Feature>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
        public bool IsOnSale => DiscountPrice.HasValue && DiscountPrice < Price;
    }

    public class Feature
    {
        public int Id { get; set; }
        [Required]
        public string FeatureName { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }

    public class Review
    {
        public int Id { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;
        public string UserName { get; set; } = "Anonymous";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "Guest";
        public string? CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "Credit Card";
        public string TransactionId { get; set; } = "N/A";
        public string Status { get; set; } = "Pending"; // Pending, Paid, Shipped, Cancelled
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class User
    {
        [Key]
        [Required]
        public string UserID { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsLocked { get; set; } = false;
        public bool IsActive { get; set; } = true; // Suspend/Activate
        public int FailedAttempts { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public string Role { get; set; } = "Customer"; // Super Admin, Admin, Customer
        public int? BranchId { get; set; }
        public virtual Branch? Branch { get; set; }
        // Permission flags (for Admins)
        public bool CanAccessInventory { get; set; } = true;
        public bool CanAccessBilling { get; set; } = true;
        public bool CanDownloadReports { get; set; } = false;
        public bool CanManageUsers { get; set; } = false;
    }

    public class Branch
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public string? ManagerId { get; set; }
        public virtual User? Manager { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class AuditLog
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "System";
        public string UserName { get; set; } = "System";
        public string Action { get; set; } = null!;       // e.g. "Delete", "Update", "Create"
        public string Module { get; set; } = null!;       // e.g. "Product", "User", "Order"
        public string Details { get; set; } = null!;      // Human readable description
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? IpAddress { get; set; }
    }

    public class SystemSettings
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = "Electric Gadget Store";
        public string Currency { get; set; } = "BDT";
        public string Timezone { get; set; } = "Asia/Dhaka";
        public string Language { get; set; } = "English";
        public string? LogoUrl { get; set; }
        public string? SupportEmail { get; set; }
        public string? Phone { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
