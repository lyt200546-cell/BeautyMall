namespace MZWlyt.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tb_user", "confirmPassword");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tb_user", "confirmPassword", c => c.String(nullable: false));
        }
    }
}
