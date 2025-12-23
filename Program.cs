using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PemitManagement.Data;
using PemitManagement.Data.Seed;
using PemitManagement.Identity;
using PemitManagement.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    ));

builder.Services.AddSingleton(TimeProvider.System);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    })
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreateObservation",
        p => p.RequireClaim("permission", "create_observations"));

    options.AddPolicy("DeleteObservation",
        p => p.RequireClaim("permission", "delete_observations"));

    options.AddPolicy("CreatePermit",
        p => p.RequireClaim("permission", "create_permits"));

    options.AddPolicy("EditPermit",
        p => p.RequireClaim("permission", "edit_permits"));

    options.AddPolicy("DeletePermit",
        p => p.RequireClaim("permission", "delete_permits"));

    options.AddPolicy("ManageLocations",
        p => p.RequireClaim("permission", "manage_locations"));

    options.AddPolicy("ManageViolations",
        p => p.RequireClaim("permission", "manage_violations"));
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<PermissionClaimService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await PermissionSeeder.SeedAsync(db);
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
