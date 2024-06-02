using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using MyBusiness.Models;
using MyBusiness.Repositories;
using MyBusiness.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(connectionString));

builder.Services.AddScoped<IDbProvider, DbProvider>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddTelerikBlazor();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
});

#region Localization

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var cultureInfo = new CultureInfo("ru-RU");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
CultureInfo.CurrentCulture = cultureInfo;
CultureInfo.CurrentUICulture = cultureInfo;

#endregion

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();