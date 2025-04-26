using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Academia1.Models;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do Entity Framework e conex�o com banco de dados
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )));

// Configura��o do Identity
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters =
        "a�bcde�fghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ "; // adicione o espa�o se quiser
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Fun��o ass�ncrona para criar as roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.EnsurePopulated(app);
}

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
