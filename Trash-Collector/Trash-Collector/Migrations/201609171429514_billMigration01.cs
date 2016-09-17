namespace Trash_Collector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class billMigration01 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Bill", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Bill");
        }
    }
}
