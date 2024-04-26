using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Areas.Identity.Data;
using WebApplication3.Models;
using WebApplication3.ViewModels;

namespace WebApplication3.Areas.Identity.Data;

public class AgiliFoodDbContext : IdentityDbContext<Usuario>
{
    public AgiliFoodDbContext(DbContextOptions<AgiliFoodDbContext> options)
        : base(options)
    {
    }


    public DbSet<Produto> Produtos { get; set; } = null!;

    public DbSet<Pedido> Pedidos { get; set; } = null!;

    public DbSet<DetalhesPedido> DetalhesPedidos { get; set; } = null!;




    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

public DbSet<WebApplication3.ViewModels.FechamentoMensalGeralDataModel> RankingPedidosDataModel { get; set; } = default!;
}
