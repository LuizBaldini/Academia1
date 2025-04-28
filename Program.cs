using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Academia1.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework e conexão com banco de dadoss
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )));

// Configuração do Identity
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters =
        "aãbcdeéfghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ "; // adicione o espaço se quiser
});

builder.Services.AddControllersWithViews();
// Configurar o caminho da página de acesso negado
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied"; // <-- aqui define para onde vai quando acesso é negado
});

var app = builder.Build();

// Função assíncrona para criar as roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    //await SeedData.EnsurePopulated(app);
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
