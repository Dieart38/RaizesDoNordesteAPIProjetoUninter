using BCrypt.Net;
using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.Domain.Enums;

namespace RaizesNordeste.API.Infrastructure
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            // Verificar se já existem dados
            if (context.Unidades.Any())
            {
                Console.WriteLine("✅ Banco já possui dados. Nada a fazer.");
                return;
            }

            Console.WriteLine("📦 Inicializando banco com dados de teste...");

            try
            {
                // ========== 1. UNIDADES ==========
                var unidadeRecife = new Unidade
                {
                    Id = Guid.NewGuid(),
                    Nome = "Raízes Recife - Boa Viagem",
                    Endereco = "Av. Boa Viagem, 1234 - Recife/PE",
                    Ativa = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var unidadeSP = new Unidade
                {
                    Id = Guid.NewGuid(),
                    Nome = "Raízes São Paulo - Paulista",
                    Endereco = "Av. Paulista, 1000 - São Paulo/SP",
                    Ativa = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await context.Unidades.AddRangeAsync(unidadeRecife, unidadeSP);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Unidades criadas");

                // ========== 2. PRODUTOS ==========
                var tapioca = new Produto
                {
                    Id = Guid.NewGuid(),
                    Nome = "Tapioca de Carne Seca",
                    Descricao = "Tapioca recheada com carne seca desfiada, queijo coalho e manteiga de garrafa",
                    Preco = 18.90m,
                    IsSazonal = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var cuscuz = new Produto
                {
                    Id = Guid.NewGuid(),
                    Nome = "Cuscuz Nordestino Completo",
                    Descricao = "Cuscuz com ovo, queijo coalho, manteiga e carne seca",
                    Preco = 22.50m,
                    IsSazonal = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var suco = new Produto
                {
                    Id = Guid.NewGuid(),
                    Nome = "Suco de Caju Natural",
                    Descricao = "Suco de caju feito na hora",
                    Preco = 8.90m,
                    IsSazonal = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await context.Produtos.AddRangeAsync(tapioca, cuscuz, suco);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Produtos criados");

                // ========== 3. ESTOQUES ==========
                var estoque1 = new Estoque
                {
                    Id = Guid.NewGuid(),
                    UnidadeId = unidadeRecife.Id,
                    ProdutoId = tapioca.Id,
                    Quantidade = 50
                };

                var estoque2 = new Estoque
                {
                    Id = Guid.NewGuid(),
                    UnidadeId = unidadeRecife.Id,
                    ProdutoId = cuscuz.Id,
                    Quantidade = 30
                };

                var estoque3 = new Estoque
                {
                    Id = Guid.NewGuid(),
                    UnidadeId = unidadeRecife.Id,
                    ProdutoId = suco.Id,
                    Quantidade = 100
                };

                var estoque4 = new Estoque
                {
                    Id = Guid.NewGuid(),
                    UnidadeId = unidadeSP.Id,
                    ProdutoId = tapioca.Id,
                    Quantidade = 10
                };

                var estoque5 = new Estoque
                {
                    Id = Guid.NewGuid(),
                    UnidadeId = unidadeSP.Id,
                    ProdutoId = suco.Id,
                    Quantidade = 50
                };

                await context.Estoques.AddRangeAsync(estoque1, estoque2, estoque3, estoque4, estoque5);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Estoques criados");

                // ========== 4. USUÁRIO ADMIN ==========
                var admin = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nome = "Administrador",
                    Email = "admin@raizes.com",
                    Cpf = null,
                    Perfil = PerfilUsuario.ADMIN,
                    ConsentimentoLGPD = true,
                    DataConsentimento = DateTime.UtcNow,
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    PontosFidelidade = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await context.Usuarios.AddAsync(admin);
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Usuário Admin criado");

                Console.WriteLine("🎉 Banco de dados inicializado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro ao inicializar banco: {ex.Message}");
                Console.WriteLine($"Detalhes: {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}