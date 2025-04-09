using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Academia.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework e conexão com banco de dados
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )));

// Configuração do Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Função assíncrona para criar as roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // Chamamos uma função assíncrona
    await CreateRolesAsync(roleManager);
}

async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
{
    if (!await roleManager.RoleExistsAsync("Personal"))
    {
        await roleManager.CreateAsync(new IdentityRole("Personal"));
    }

    if (!await roleManager.RoleExistsAsync("Aluno"))
    {
        await roleManager.CreateAsync(new IdentityRole("Aluno"));
    }
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
    pattern: "{controller=Account}/{action=Login}/{id?}");


app.Run();
