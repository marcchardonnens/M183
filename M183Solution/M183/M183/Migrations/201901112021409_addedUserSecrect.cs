namespace M183.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedUserSecrect : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "GoogleAuthSecret", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "GoogleAuthSecret");
        }
    }
}
