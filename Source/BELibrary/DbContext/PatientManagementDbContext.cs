namespace BELibrary.DbContext
{
    using BELibrary.Entity;
    using System.Data.Entity;

    public partial class HospitalManagementDbContext : DbContext
    {
        public HospitalManagementDbContext()
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Account> Accounts { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<DetailItemSite> DetailItemSites { get; set; }

        public virtual DbSet<DetailRecord> DetailRecords { get; set; }

        public virtual DbSet<Item> Items { get; set; }

        public virtual DbSet<ItemSupply> ItemSupplies { get; set; }

        public virtual DbSet<Movement> Movements { get; set; }

        public virtual DbSet<Location> Locations { get; set; }

        public virtual DbSet<ItemSite> ItemSites { get; set; }

        public virtual DbSet<Record> Records { get; set; }

        public virtual DbSet<Attachment> Attachments { get; set; }

        public virtual DbSet<AttachmentAssign> AttachmentAssigns { get; set; }

        public virtual DbSet<Faculty> Faculties { get; set; }

        public virtual DbSet<Director> Directors { get; set; }

        public virtual DbSet<LocationRecord> LocationRecords { get; set; }

        public virtual DbSet<UserVerification> UserVerifications { get; set; }

        public virtual DbSet<DirectorWork> DirectorWorks { get; set; }

        public virtual DbSet<LocationDirector> LocationDirectors { get; set; }

        public virtual DbSet<Article> Articles { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Items)
                .WithRequired(e => e.Category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Movement>()
                .HasMany(e => e.DetailItemSites)
                .WithRequired(e => e.Movement)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Location>()
                .Property(e => e.IndentificationCardId)
                .IsUnicode(false);

            modelBuilder.Entity<Location>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Location>()
                .HasMany(e => e.ItemSupplies)
                .WithRequired(e => e.Location)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Record>()
                .HasMany(e => e.DetailRecords)
                .WithRequired(e => e.Record)
                .WillCascadeOnDelete(false);
        }
    }
}