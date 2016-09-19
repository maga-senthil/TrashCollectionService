namespace Trash_Collector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class latlngMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Latitude", c => c.String());
            AddColumn("dbo.Customers", "Longitude", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Longitude");
            DropColumn("dbo.Customers", "Latitude");
        }
    }
}
