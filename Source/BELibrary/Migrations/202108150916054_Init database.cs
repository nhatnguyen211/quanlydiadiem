namespace BELibrary.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initdatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Account",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    FullName = c.String(maxLength: 50),
                    Phone = c.String(maxLength: 15),
                    UserName = c.String(maxLength: 50),
                    LinkAvatar = c.String(maxLength: 250),
                    Gender = c.Boolean(nullable: false),
                    Password = c.String(maxLength: 250),
                    Role = c.Int(nullable: false),
                    IsDeleted = c.Boolean(nullable: false),
                    LocationId = c.Guid(),
                    DirectorId = c.Guid(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Director", t => t.DirectorId)
                .ForeignKey("dbo.Location", t => t.LocationId)
                .Index(t => t.LocationId)
                .Index(t => t.DirectorId);

            CreateTable(
                "dbo.Director",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(),
                    Avatar = c.String(),
                    Descriptions = c.String(),
                    Address = c.String(),
                    Gender = c.Boolean(nullable: false),
                    Phone = c.String(),
                    Email = c.String(),
                    IsDelete = c.Boolean(nullable: false),
                    FacultyId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculty", t => t.FacultyId, cascadeDelete: true)
                .Index(t => t.FacultyId);

            CreateTable(
                "dbo.Faculty",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Location",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    FullName = c.String(nullable: false, maxLength: 100),
                    DateOfBirth = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    Address = c.String(nullable: false, maxLength: 250),
                    Gender = c.Boolean(nullable: false),
                    IndentificationCardId = c.String(maxLength: 20, unicode: false),
                    Phone = c.String(nullable: false, maxLength: 15, unicode: false),
                    Status = c.Boolean(nullable: false),
                    ImageProfile = c.String(),
                    PatientCode = c.String(nullable: false),
                    JoinDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    IndentificationCardDate = c.DateTime(nullable: false),
                    Job = c.String(),
                    WorkPlace = c.String(),
                    HistoryOfIllnessFamily = c.String(),
                    HistoryOfIllnessYourself = c.String(),
                    IsDeleted = c.Boolean(nullable: false),
                    Email = c.String(maxLength: 250),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.ItemSupplies",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    DateOfHire = c.DateTime(nullable: false),
                    Status = c.Int(nullable: false),
                    Amount = c.Int(nullable: false),
                    ItemId = c.Guid(nullable: false),
                    LocationId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Item", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.Location", t => t.LocationId)
                .Index(t => t.ItemId)
                .Index(t => t.LocationId);

            CreateTable(
                "dbo.Item",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 250),
                    Amount = c.Int(nullable: false),
                    Description = c.String(),
                    CategoryId = c.Guid(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Category", t => t.CategoryId)
                .Index(t => t.CategoryId);

            CreateTable(
                "dbo.Category",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 200),
                    Unit = c.String(nullable: false, maxLength: 50),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.AttachmentAssigns",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    AttachmentId = c.Guid(nullable: false),
                    DetailRecordId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attachments", t => t.AttachmentId, cascadeDelete: true)
                .ForeignKey("dbo.DetailRecord", t => t.DetailRecordId, cascadeDelete: true)
                .Index(t => t.AttachmentId)
                .Index(t => t.DetailRecordId);

            CreateTable(
                "dbo.Attachments",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(),
                    Type = c.String(),
                    Url = c.String(),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.DetailRecord",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    DiseaseName = c.String(nullable: false, maxLength: 200),
                    Note = c.String(),
                    Result = c.String(),
                    Status = c.Boolean(nullable: false),
                    DirectorId = c.Guid(),
                    FacultyId = c.Guid(),
                    Process = c.Int(nullable: false),
                    RecordId = c.Guid(nullable: false),
                    IsMainRecord = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Faculty", t => t.FacultyId)
                .ForeignKey("dbo.Record", t => t.RecordId)
                .Index(t => t.FacultyId)
                .Index(t => t.RecordId);

            CreateTable(
                "dbo.Record",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                    DirectorId = c.Guid(),
                    Note = c.String(),
                    Result = c.String(),
                    StatusRecord = c.Int(nullable: false),
                    IsDelete = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Director", t => t.DirectorId)
                .Index(t => t.DirectorId);

            CreateTable(
                "dbo.DetailItemSite",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Amount = c.Int(nullable: false),
                    Unit = c.String(nullable: false, maxLength: 50),
                    Note = c.String(nullable: false),
                    MovementId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Movement", t => t.MovementId)
                .Index(t => t.MovementId);

            CreateTable(
                "dbo.Movement",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Name = c.String(nullable: false, maxLength: 200),
                    Description = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.DirectorWork",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DirectorId = c.Guid(nullable: false),
                    LocationId = c.Guid(nullable: false),
                    ScheduleBook = c.DateTime(nullable: false),
                    Status = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Director", t => t.DirectorId, cascadeDelete: true)
                .ForeignKey("dbo.Location", t => t.LocationId, cascadeDelete: true)
                .Index(t => t.DirectorId)
                .Index(t => t.LocationId);

            CreateTable(
                "dbo.LocationDirector",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DirectorId = c.Guid(nullable: false),
                    LocationId = c.Guid(nullable: false),
                    Status = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Director", t => t.DirectorId, cascadeDelete: true)
                .ForeignKey("dbo.Location", t => t.LocationId, cascadeDelete: true)
                .Index(t => t.DirectorId)
                .Index(t => t.LocationId);

            CreateTable(
                "dbo.LocationRecord",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    Title = c.String(),
                    TestDate = c.DateTime(nullable: false),
                    BloodVessel = c.Double(nullable: false),
                    BodyTemperature = c.Double(nullable: false),
                    Height = c.Double(nullable: false),
                    Weight = c.Double(nullable: false),
                    Breathing = c.Double(nullable: false),
                    Event = c.String(),
                    Event2 = c.String(),
                    ForManage2 = c.String(),
                    ForManage = c.String(),
                    ForDate2 = c.String(),
                    ForDate = c.String(),
                    EyePressureRight = c.String(),
                    EyePressureLeft = c.String(),
                    Formation = c.String(),
                    Activity = c.String(),
                    Activity2 = c.String(),
                    Activity3 = c.String(),
                    Status = c.Boolean(nullable: false),
                    IsDelete = c.Boolean(nullable: false),
                    DirectorId = c.Guid(),
                    RecordId = c.Guid(nullable: false),
                    LocationId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Director", t => t.DirectorId)
                .ForeignKey("dbo.Location", t => t.LocationId, cascadeDelete: true)
                .ForeignKey("dbo.Record", t => t.RecordId, cascadeDelete: true)
                .Index(t => t.DirectorId)
                .Index(t => t.RecordId)
                .Index(t => t.LocationId);

            CreateTable(
                "dbo.ItemSite",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    DetailItemSiteId = c.Guid(nullable: false),
                    DetailRecordId = c.Guid(nullable: false),
                    CreatedDate = c.DateTime(nullable: false),
                    CreatedBy = c.String(),
                    ModifiedDate = c.DateTime(nullable: false),
                    ModifiedBy = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DetailItemSite", t => t.DetailItemSiteId, cascadeDelete: true)
                .ForeignKey("dbo.DetailRecord", t => t.DetailRecordId, cascadeDelete: true)
                .Index(t => t.DetailItemSiteId)
                .Index(t => t.DetailRecordId);

            CreateTable(
                "dbo.UserVerification",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Mode = c.String(),
                    Token = c.String(),
                    TokenExpirationDate = c.DateTime(),
                    VerificationCode = c.String(),
                    CodeExpirationDate = c.DateTime(),
                    CreatedDate = c.DateTime(nullable: false),
                    Status = c.Int(),
                    Link = c.Int(nullable: false),
                    AccountId = c.Int(),
                    Email = c.String(),
                    DeletedDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.ItemSite", "DetailRecordId", "dbo.DetailRecord");
            DropForeignKey("dbo.ItemSite", "DetailItemSiteId", "dbo.DetailItemSite");
            DropForeignKey("dbo.LocationRecord", "RecordId", "dbo.Record");
            DropForeignKey("dbo.LocationRecord", "LocationId", "dbo.Location");
            DropForeignKey("dbo.LocationRecord", "DirectorId", "dbo.Director");
            DropForeignKey("dbo.LocationDirector", "LocationId", "dbo.Location");
            DropForeignKey("dbo.LocationDirector", "DirectorId", "dbo.Director");
            DropForeignKey("dbo.DirectorWork", "LocationId", "dbo.Location");
            DropForeignKey("dbo.DirectorWork", "DirectorId", "dbo.Director");
            DropForeignKey("dbo.DetailItemSite", "MovementId", "dbo.Movement");
            DropForeignKey("dbo.AttachmentAssigns", "DetailRecordId", "dbo.DetailRecord");
            DropForeignKey("dbo.Record", "DirectorId", "dbo.Director");
            DropForeignKey("dbo.DetailRecord", "RecordId", "dbo.Record");
            DropForeignKey("dbo.DetailRecord", "FacultyId", "dbo.Faculty");
            DropForeignKey("dbo.AttachmentAssigns", "AttachmentId", "dbo.Attachments");
            DropForeignKey("dbo.ItemSupplies", "LocationId", "dbo.Location");
            DropForeignKey("dbo.ItemSupplies", "ItemId", "dbo.Item");
            DropForeignKey("dbo.Item", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.Account", "LocationId", "dbo.Location");
            DropForeignKey("dbo.Account", "DirectorId", "dbo.Director");
            DropForeignKey("dbo.Director", "FacultyId", "dbo.Faculty");
            DropIndex("dbo.ItemSite", new[] { "DetailRecordId" });
            DropIndex("dbo.ItemSite", new[] { "DetailItemSiteId" });
            DropIndex("dbo.LocationRecord", new[] { "LocationId" });
            DropIndex("dbo.LocationRecord", new[] { "RecordId" });
            DropIndex("dbo.LocationRecord", new[] { "DirectorId" });
            DropIndex("dbo.LocationDirector", new[] { "LocationId" });
            DropIndex("dbo.LocationDirector", new[] { "DirectorId" });
            DropIndex("dbo.DirectorWork", new[] { "LocationId" });
            DropIndex("dbo.DirectorWork", new[] { "DirectorId" });
            DropIndex("dbo.DetailItemSite", new[] { "MovementId" });
            DropIndex("dbo.Record", new[] { "DirectorId" });
            DropIndex("dbo.DetailRecord", new[] { "RecordId" });
            DropIndex("dbo.DetailRecord", new[] { "FacultyId" });
            DropIndex("dbo.AttachmentAssigns", new[] { "DetailRecordId" });
            DropIndex("dbo.AttachmentAssigns", new[] { "AttachmentId" });
            DropIndex("dbo.Item", new[] { "CategoryId" });
            DropIndex("dbo.ItemSupplies", new[] { "LocationId" });
            DropIndex("dbo.ItemSupplies", new[] { "ItemId" });
            DropIndex("dbo.Director", new[] { "FacultyId" });
            DropIndex("dbo.Account", new[] { "DirectorId" });
            DropIndex("dbo.Account", new[] { "LocationId" });
            DropTable("dbo.UserVerification");
            DropTable("dbo.ItemSite");
            DropTable("dbo.LocationRecord");
            DropTable("dbo.LocationDirector");
            DropTable("dbo.DirectorWork");
            DropTable("dbo.Movement");
            DropTable("dbo.DetailItemSite");
            DropTable("dbo.Record");
            DropTable("dbo.DetailRecord");
            DropTable("dbo.Attachments");
            DropTable("dbo.AttachmentAssigns");
            DropTable("dbo.Category");
            DropTable("dbo.Item");
            DropTable("dbo.ItemSupplies");
            DropTable("dbo.Location");
            DropTable("dbo.Faculty");
            DropTable("dbo.Director");
            DropTable("dbo.Account");
        }
    }
}