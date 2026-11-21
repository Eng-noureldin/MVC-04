using Microsoft.EntityFrameworkCore;
using Session04_MVC.BLL;
using Session04_MVC.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ECommerceSession04Db"));

// Register Unit of Work in DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!context.Categories.Any())
    {
        var c1 = new Category { Name = "Gaming Gear", Description = "Consoles and PC gear" };
        var c2 = new Category { Name = "Smart Devices", Description = "Watches, tablets, and phones" };
        context.Categories.AddRange(c1, c2);
        context.SaveChanges();

        context.Products.AddRange(
            new Product { Name = "Mechanical Keyboard RGB", Price = 129.99m, Description = "High-speed switches", StockQuantity = 15, CategoryId = c1.Id },
            new Product { Name = "Gaming Headset 7.1", Price = 79.50m, Description = "Surround sound headset", StockQuantity = 30, CategoryId = c1.Id },
            new Product { Name = "Smartwatch Ultra", Price = 299.00m, Description = "Fitness and sleep tracking", StockQuantity = 20, CategoryId = c2.Id }
        );
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Required to serve uploaded files from wwwroot
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();
