using Microsoft.EntityFrameworkCore;
using System.Configuration;
using WebApplication3.Models;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AgiliFoodDbContextConnection") ?? throw new InvalidOperationException("Connection string 'UsuarioDbContextConnection' not found.");

builder.Services.AddDbContext<AgiliFoodDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<Usuario>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AgiliFoodDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Funcionario", policy => policy.RequireRole("Funcionario"));
    options.AddPolicy("Financeiro", policy => policy.RequireRole("Financeiro"));
    options.AddPolicy("Fornecedor", policy => policy.RequireRole("Fornecedor"));
    options.AddPolicy("FornecedorAtivo", policy => policy.RequireRole("FornecedorAtivo"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Funcionario", "Financeiro", "Fornecedor", "FornecedorAtivo"};

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
