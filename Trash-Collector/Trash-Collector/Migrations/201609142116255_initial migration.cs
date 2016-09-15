namespace Trash_Collector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialmigration : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Customers", name: "Register_Id", newName: "Email");
            RenameIndex(table: "dbo.Customers", name: "IX_Register_Id", newName: "IX_Email");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Customers", name: "IX_Email", newName: "IX_Register_Id");
            RenameColumn(table: "dbo.Customers", name: "Email", newName: "Register_Id");
        }
    }
}
