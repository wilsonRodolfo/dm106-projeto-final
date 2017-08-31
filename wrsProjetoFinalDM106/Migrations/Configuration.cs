namespace wrsProjetoFinalDM106.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using wrsProjetoFinalDM106.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<wrsProjetoFinalDM106.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "wrsProjetoFinalDM106.Models.ApplicationDbContext";
        }

        protected override void Seed(wrsProjetoFinalDM106.Models.ApplicationDbContext context)
        {
            context.Products.AddOrUpdate(
                p => p.Id,

                new Product {
                    Id = 1, nome = "produto 1", descricao = "descrição produto 1", cor = "preto", modelo = "modelo produto 1", codigo = "COD1",
                    preco = 10, peso = 10, altura = 10, largura = 10, comprimento = 10, urlImagem = "www.image1.com/produto"
                },

                new Product { Id = 2, nome = "produto 2", descricao = "descrição produto 2", cor = "branco", modelo = "modelo produto 2", codigo = "COD2",
                    preco = 20, peso = 20, altura = 20, largura = 20, comprimento = 20, urlImagem = "www.image2.com/produto"
                },

                new Product { Id = 3, nome = "produto 3", descricao = "descrição produto 3", cor = "azul", modelo = "modelo produto 3", codigo = "COD3",
                    preco = 30, peso = 30, altura = 30, largura = 30, comprimento = 30, urlImagem = "www.image3.com/produto"
                }
            );
        }
    }
}
