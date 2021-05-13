namespace SpaceOrganizing.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        TaskId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        UserId = c.String(maxLength: 128),
                        UserId2 = c.String(),
                        Deadline = c.DateTime(nullable: false),
                        GroupId = c.Int(nullable: false),
                        Done = c.Boolean(nullable: false),
                        User2_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.TaskId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.User2_Id)
                .Index(t => t.UserId)
                .Index(t => t.User2_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "User2_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tasks", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Tasks", new[] { "User2_Id" });
            DropIndex("dbo.Tasks", new[] { "UserId" });
            DropTable("dbo.Tasks");
        }
    }
}
