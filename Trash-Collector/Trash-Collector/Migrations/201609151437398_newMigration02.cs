namespace Trash_Collector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newMigration02 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customers", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Customers", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.Customers", "StreetAddress", c => c.String(nullable: false));
            AlterColumn("dbo.Customers", "City", c => c.String(nullable: false));
            AlterColumn("dbo.Customers", "state", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "state", c => c.String());
            AlterColumn("dbo.Customers", "City", c => c.String());
            AlterColumn("dbo.Customers", "StreetAddress", c => c.String());
            AlterColumn("dbo.Customers", "LastName", c => c.String());
            AlterColumn("dbo.Customers", "FirstName", c => c.String());
        }
    }
}
