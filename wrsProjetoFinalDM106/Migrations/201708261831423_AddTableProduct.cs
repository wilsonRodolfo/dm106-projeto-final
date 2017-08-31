namespace wrsProjetoFinalDM106.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false),
                        descricao = c.String(),
                        cor = c.String(),
                        modelo = c.String(nullable: false),
                        codigo = c.String(nullable: false, maxLength: 8),
                        preco = c.Decimal(nullable: false, precision: 18, scale: 2),
                        peso = c.Int(nullable: false),
                        altura = c.Int(nullable: false),
                        largura = c.Int(nullable: false),
                        comprimento = c.Int(nullable: false),
                        diamentro = c.Int(nullable: false),
                        urlImagem = c.String(maxLength: 80),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Products");
        }
    }
}
