using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DoctorTalkWebApp.Data;
using DoctorTalkWebApp.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using DoctorTalkWebApp.Data.Interfaces;
using DoctorTalkWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DoctorTalkWebAppContextConnection") ?? throw new InvalidOperationException("Connection string 'DoctorTalkWebAppContextConnection' not found.");

builder.Services.AddDbContext<DoctorTalkWebAppContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<DoctorTalkWebAppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DoctorTalkWebAppContext>();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IForum, ForumService>();
builder.Services.AddScoped<IPost, PostService>();

builder.Services.AddTransient<DataSeeder>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await dataSeeder.SeedSuperUser(); // Call the seed method here
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
