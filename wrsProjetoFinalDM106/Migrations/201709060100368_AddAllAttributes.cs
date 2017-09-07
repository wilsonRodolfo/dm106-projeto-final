namespace wrsProjetoFinalDM106.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAllAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "userEmail", c => c.String());
            AddColumn("dbo.Orders", "freightPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.OrderItems", "Quantidade", c => c.Int(nullable: false));
            DropColumn("dbo.Orders", "userName");
            DropColumn("dbo.Orders", "precoFrete");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "precoFrete", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "userName", c => c.String());
            DropColumn("dbo.OrderItems", "Quantidade");
            DropColumn("dbo.Orders", "freightPrice");
            DropColumn("dbo.Orders", "userEmail");
        }
    }
}
