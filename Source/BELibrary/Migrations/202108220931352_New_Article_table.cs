namespace BELibrary.Migrations
{
    using BELibrary.Entity;
    using System.Data.Entity.Migrations;
    using System.Xml.Linq;

    public partial class New_Article_table : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE Locationrecord RENAME COLUMN BloodVessel TO Test");
            Sql("ALTER TABLE Locationrecord RENAME COLUMN BodyTemperature TO Test2");
            Sql("ALTER TABLE Locationrecord RENAME COLUMN Height TO Test3");
            Sql("ALTER TABLE Locationrecord RENAME COLUMN Weight TO Test4");
            Sql("ALTER TABLE Locationrecord RENAME COLUMN Breathing TO Test5");
        }
        public override void Down()
        {
            Sql("ALTER TABLE Locationrecord RENAME COLUMN Test TO BloodVessel");
            Sql("ALTER TABLE Locationrecord RENAME COLUMN Test2 TO BodyTemperature");
            Sql("ALTER TABLE Locationrecord RENAME COLUMN Test3 TO Height");
            Sql("ALTER TABLE Locationrecord RENAME COLUMN Test4 TO Weight");
            Sql("ALTER TABLE Locationrecord RENAME COLUMN Test5 TO Breathing");
        }
    }
}