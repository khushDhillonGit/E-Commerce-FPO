using JattanaNursury.Data;
using JattanaNursury.Models;
using JattanaNursury.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

//setup serilog
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
var connectionString = builder.Configuration["DefaultConnection"];


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    { var sqlBuilder = new SqlConnectionStringBuilder(connectionString);
        options.UseSqlServer(sqlBuilder.ConnectionString); options.UseLazyLoadingProxies(); });

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultUI().AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailSender, EmailSender>();

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

    app.Use(async (context, next) =>
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            //Log exception
        }
    });
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

