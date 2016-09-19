namespace Trash_Collector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class noCoordinateMigration : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Customers", "Coordinates");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "Coordinates", c => c.String());
        }
    }
}
