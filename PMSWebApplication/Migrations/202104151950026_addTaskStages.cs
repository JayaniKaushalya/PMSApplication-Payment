namespace PMSWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTaskStages : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "TaskStages", c => c.String(nullable: false));
            DropColumn("dbo.Payments", "InvoiceNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "InvoiceNo", c => c.Int(nullable: false));
            DropColumn("dbo.Payments", "TaskStages");
        }
    }
}
