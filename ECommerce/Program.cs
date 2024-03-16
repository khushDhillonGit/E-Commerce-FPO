using ECommerce.Controllers;
using ECommerce.Data;
using ECommerce.Data.Models;
using ECommerce.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Cryptography.X509Certificates;


var builder = WebApplication.CreateBuilder(args);

//setup serilog
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
var databaseProvide = builder.Configuration["databaseProvider"] ?? "SqlServer";

builder.Services.AddDbContext<ApplicationDbContext>(options => _ = databaseProvide switch
{
    "SqlServer" =>
    options.UseSqlServer(builder.Configuration["SqlServerCnn"], x => x.MigrationsAssembly("SqlServerMigrations")),

    "Postgresql" =>
    options.UseNpgsql(builder.Configuration["PostgresqlCnn"], x => x.MigrationsAssembly("PostgresqlMigrations")),

    _ => throw new NotSupportedException("Provider not recognised")
});


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultUI().AddDefaultTokenProviders();
builder.Services.AddTransient<ImageUtility>();
//builder.Services.AddTransient<IClaimsTransformation, UserClaimsTransformation>();
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddSession();

try
{
    Log.Information("Application Starting Up");
    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error?code=500");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseSession();
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    //All requests will be logged
    app.UseSerilogRequestLogging();

    app.UseRouting();

    //using (var scope = app.Services.CreateScope()) 
    //{
    //    await SeedDatabase.Initialize(scope.ServiceProvider);
    //}

    app.UseAuthentication();
    app.UseAuthorization();

    //app.Use(async (context, next) =>
    //{
    //    try
    //    {
    //        await next(context);
    //    }
    //    catch (Exception ex)
    //    {
    //        //Log exception
    //        Log.Logger.Error(ex, "{Date}: {Message}", DateTimeOffset.UtcNow, ex.Message);
    //    }
    //});
    app.UseStatusCodePagesWithRedirects("/Home/Error?code={0}");
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Fatal: Application failed to start");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { }

