namespace PMSWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changetaskandproject : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Attachments", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Attachments", "Type", c => c.String());
        }
    }
}
