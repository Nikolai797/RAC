using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentACar.Data.Data;
using RentACar.Data.Models;
using RentACar.Services.Implementations;
using RentACar.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IReservationService, ReservationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();   // Detailed errors for debugging
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed roles, admin user, and Ferrari cars
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Seed roles
    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Seed admin user
    string adminEmail = "admin@rentacar.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "Adminov",
            EGN = "0000000000",
            PhoneNumber = "0000000000"
        };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Seed Ferrari cars if none exist
    if (!context.Cars.Any())
    {
        var ferraris = new List<Car>
        {
            new Car { Brand = "Ferrari", Model = "488 GTB", Year = 2021, Seats = 2, PricePerDay = 350, Description = "Twin-turbo V8, 670 hp, 0-100 km/h in 3.0s" },
            new Car { Brand = "Ferrari", Model = "F8 Tributo", Year = 2022, Seats = 2, PricePerDay = 380, Description = "720 hp V8,极致性能" },
            new Car { Brand = "Ferrari", Model = "SF90 Stradale", Year = 2023, Seats = 2, PricePerDay = 450, Description = "Hybrid V8, 1000 hp, AWD" },
            new Car { Brand = "Ferrari", Model = "Roma", Year = 2022, Seats = 4, PricePerDay = 320, Description = "Elegant grand tourer, V8 turbo" },
            new Car { Brand = "Ferrari", Model = "Portofino M", Year = 2021, Seats = 4, PricePerDay = 300, Description = "Convertible grand tourer, 620 hp" },
            new Car { Brand = "Ferrari", Model = "812 Superfast", Year = 2020, Seats = 2, PricePerDay = 500, Description = "V12, 789 hp, naturally aspirated" }
        };
        await context.Cars.AddRangeAsync(ferraris);
        await context.SaveChangesAsync();
    }
}

app.Run();