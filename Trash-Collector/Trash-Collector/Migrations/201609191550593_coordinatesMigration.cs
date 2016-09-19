namespace Trash_Collector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class coordinatesMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Coordinates", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Coordinates");
        }
    }
}
