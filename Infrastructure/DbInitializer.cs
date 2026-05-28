using BCrypt.Net;
using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.Domain.Enums;

namespace RaizesNordeste.API.Infrastructure
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            // Verificar se já existem unidades
            if (context.Unidades.Any())
                return;

            // Criar unidades
            var unidadeRecife = new Unidade
            {
                Nome = "Raízes Recife - Boa Viagem",
                Endereco = "Av. Boa Viagem, 1234 - Recife/PE",
                Ativa = true
            };

            var unidadeSP = new Unidade
            {
                Nome = "Raízes São Paulo - Paulista",
                Endereco = "Av. Paulista, 1000 - São Paulo/SP",
                Ativa = true
            };

            await context.Unidades.AddRangeAsync(unidadeRecife, unidadeSP);
            await context.SaveChangesAsync();

            // Criar produtos
            var tapiocaCarneSeco = new Produto
            {
                Nome = "Tapioca de Carne Seca",
                Descricao = "Tapioca recheada com carne seca desfiada, queijo coalho e manteiga de garrafa",
                Preco = 18.90m
            };

            var cuscuzNordestino = new Produto
            {
                Nome = "Cuscuz Nordestino Completo",
                Descricao = "Cuscuz com ovo, queijo coalho, manteiga e carne seca",
                Preco = 22.50m
            };

            var sucoCaju = new Produto
            {
                Nome = "Suco de Caju Natural",
                Descricao = "Suco de caju feito na hora",
                Preco = 8.90m
            };

            var canjica = new Produto
            {
                Nome = "Canjica Junina",
                Descricao = "Canjica com leite de coco, canela e amendoim",
                Preco = 12.90m,
                IsSazonal = true,
                InicioSazonalidade = new DateTime(DateTime.UtcNow.Year, 6, 1),
                FimSazonalidade = new DateTime(DateTime.UtcNow.Year, 7, 31)
            };

            await context.Produtos.AddRangeAsync(tapiocaCarneSeco, cuscuzNordestino, sucoCaju, canjica);
            await context.SaveChangesAsync();

            // Criar estoque para Recife
            var estoques = new List<Estoque>
            {
                new Estoque { UnidadeId = unidadeRecife.Id, ProdutoId = tapiocaCarneSeco.Id, Quantidade = 50 },
                new Estoque { UnidadeId = unidadeRecife.Id, ProdutoId = cuscuzNordestino.Id, Quantidade = 30 },
                new Estoque { UnidadeId = unidadeRecife.Id, ProdutoId = sucoCaju.Id, Quantidade = 100 },
                new Estoque { UnidadeId = unidadeSP.Id, ProdutoId = tapiocaCarneSeco.Id, Quantidade = 10 },
                new Estoque { UnidadeId = unidadeSP.Id, ProdutoId = sucoCaju.Id, Quantidade = 50 }
            };

            await context.Estoques.AddRangeAsync(estoques);
            await context.SaveChangesAsync();

            // Criar usuário admin
            var admin = new Usuario
            {
                Nome = "Administrador",
                Email = "admin@raizes.com",
                Perfil = PerfilUsuario.ADMIN,
                ConsentimentoLGPD = true,
                DataConsentimento = DateTime.UtcNow,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PontosFidelidade = 0
            };

            await context.Usuarios.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}