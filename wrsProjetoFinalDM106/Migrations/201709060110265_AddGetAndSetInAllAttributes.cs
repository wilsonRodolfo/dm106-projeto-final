namespace wrsProjetoFinalDM106.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGetAndSetInAllAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "orderDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "deliveryDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "status", c => c.String());
            AddColumn("dbo.Orders", "orderPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "orderWeight", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "orderWeight");
            DropColumn("dbo.Orders", "orderPrice");
            DropColumn("dbo.Orders", "status");
            DropColumn("dbo.Orders", "deliveryDate");
            DropColumn("dbo.Orders", "orderDate");
        }
    }
}
