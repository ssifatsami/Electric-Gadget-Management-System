using ElectricGadget.Web.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows.Forms;
using Electric_Gadget_Management;

namespace ElectricGadget.Web
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Added Services
            builder.Services.AddScoped<ElectricGadget.Web.Repositories.IProductRepository, ElectricGadget.Web.Repositories.ProductRepository>();
            builder.Services.AddScoped<ElectricGadget.Web.Services.IProductService, ElectricGadget.Web.Services.ProductService>();

            // Session Support
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Database Initialization & Seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // 🚀 Start Web Server in a background thread
            _ = Task.Run(() => app.Run("http://localhost:5255"));

            // 🖥️ Start WinForms Application
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Assuming Form1 is your Login/Main Dashboard
            Application.Run(new Form1());
        }
    }
}
