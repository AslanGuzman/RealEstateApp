using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application;
using RealEstateApp.Infrastructure.Identity;
using RealEstateApp.Infrastructure.Identity.Contexts;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceLayerIoc(builder.Configuration);
builder.Services.AddApplicationLayerIoc();
builder.Services.AddSharedLayerIoc(builder.Configuration);
builder.Services.AddIdentityLayerIocForWebApp(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var appDb = scope.ServiceProvider
        .GetRequiredService<RealEstateAppContext>();
    await appDb.Database.MigrateAsync();

    var identityDb = scope.ServiceProvider
        .GetRequiredService<IdentityContext>();
    await identityDb.Database.MigrateAsync();
}

await app.Services.RunIdentitySeedAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.RunAsync();
