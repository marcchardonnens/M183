namespace M183.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loginlog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoginLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        TimeCreated = c.DateTime(nullable: false),
                        Success = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LoginLogs");
        }
    }
}
