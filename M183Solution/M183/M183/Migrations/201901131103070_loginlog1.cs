namespace M183.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loginlog1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoginLogs", "Email", c => c.String());
            DropColumn("dbo.LoginLogs", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LoginLogs", "UserId", c => c.String());
            DropColumn("dbo.LoginLogs", "Email");
        }
    }
}
