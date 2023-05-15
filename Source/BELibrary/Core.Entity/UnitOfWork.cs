using BELibrary.Core.Entity.Repositories;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Persistence.Repositories;

namespace BELibrary.Core.Entity
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HospitalManagementDbContext _context;

        public UnitOfWork(HospitalManagementDbContext context)
        {
            _context = context;
            Accounts = new AccountRepository(_context);
            Categories = new CategoryRepository(_context);
            DetailItemSites = new DetailItemSiteRepository(_context);
            Items = new ItemRepository(_context);
            DetailRecords = new DetailRecordRepository(_context);
            Movements = new MovementRepository(_context);
            Locations = new LocationRepository(_context);
            ItemSupplies = new ItemSupplyRepository(_context);
            ItemSites = new ItemSiteRepository(_context);
            Records = new RecordRepository(_context);
            AttachmentAssigns = new AttachmentAssignRepository(_context);
            Attachments = new AttachmentRepository(_context);
            Directors = new DirectorRepository(_context);
            Faculties = new FacultyRepository(_context);
            LocationRecords = new LocationRecordRepository(_context);
            UserVerifications = new UserVerificationRepository(_context);
            DirectorWorks = new DirectorWorkRepository(_context);
            LocationDirectors = new LocationDirectorRepository(_context);
            Articles = new ArticleRepository(_context);
        }

        public IAccountRepository Accounts { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IDetailItemSiteRepository DetailItemSites { get; private set; }
        public IItemRepository Items { get; private set; }
        public IDetailRecordRepository DetailRecords { get; private set; }
        public IMovementRepository Movements { get; private set; }
        public ILocationRepository Locations { get; private set; }
        public IItemSupplyRepository ItemSupplies { get; private set; }
        public IItemSiteRepository ItemSites { get; private set; }
        public IRecordRepository Records { get; private set; }
        public IAttachmentAssignRepository AttachmentAssigns { get; private set; }
        public IAttachmentRepository Attachments { get; private set; }
        public IFacultyRepository Faculties { get; private set; }
        public IDirectorRepository Directors { get; private set; }
        public ILocationRecordRepository LocationRecords { get; private set; }
        public IUserVerificationRepository UserVerifications { get; private set; }
        public IDirectorWorkRepository DirectorWorks { get; private set; }
        public ILocationDirectorRepository LocationDirectors { get; private set; }
        public IArticleRepository Articles { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}