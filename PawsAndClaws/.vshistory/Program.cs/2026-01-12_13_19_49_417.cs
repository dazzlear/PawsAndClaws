using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PawsAndClaws.Data;
using PawsAndClaws.Models.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var cs = builder.Configuration.GetConnectionString("PawsAndClaws")
         ?? throw new InvalidOperationException("Missing ConnectionStrings:PawsAndClaws in appsettings.json");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(cs)
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
.WithStaticAssets();

app.Run();
