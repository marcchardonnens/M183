namespace M183.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedUserSecrect1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "GoogleAuthSecret");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "GoogleAuthSecret", c => c.String());
        }
    }
}
