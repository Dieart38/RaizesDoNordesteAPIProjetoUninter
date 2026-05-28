using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.Domain.Enums;

namespace RaizesNordeste.API.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Unidade> Unidades { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<PagamentoMock> PagamentosMock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Unidade>().ToTable("Unidades");
            modelBuilder.Entity<Produto>().ToTable("Produtos");
            modelBuilder.Entity<Pedido>().ToTable("Pedidos");
            modelBuilder.Entity<ItemPedido>().ToTable("ItensPedido");
            modelBuilder.Entity<Estoque>().ToTable("Estoques");
            modelBuilder.Entity<PagamentoMock>().ToTable("PagamentosMock");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Perfil)
                .HasConversion<string>();

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Pedido>()
                .Property(p => p.CanalOrigem)
                .HasConversion<string>();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Estoque>()
                .HasIndex(e => new { e.UnidadeId, e.ProdutoId })
                .IsUnique();
        }
    }

    
    public class AppDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            
            string? connectionString = null;

            
            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddUserSecrets<Program>()
                    .Build();

                connectionString = configuration.GetConnectionString("DefaultConnection");

                if (!string.IsNullOrEmpty(connectionString))
                {
                    System.Console.WriteLine("✅ Usando connection string do User Secrets");
                }
            }
            catch
            {
                
            }

            
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

                if (!string.IsNullOrEmpty(connectionString))
                {
                    System.Console.WriteLine("✅ Usando connection string da variável de ambiente");
                }
            }

            
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Host=localhost;Database=RaizesNordesteDB;Username=postgres;Password=SUA_SENHA_AQUI";
                System.Console.WriteLine("⚠️ Usando connection string placeholder. Configure User Secrets ou variável de ambiente DB_CONNECTION_STRING");
            }

            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}