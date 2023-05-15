using BELibrary.Core.Entity.Repositories;
using BELibrary.Persistence.Repositories;
using System;

namespace BELibrary.Core.Entity
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository Accounts { get; }
        ICategoryRepository Categories { get; }
        IDetailItemSiteRepository DetailItemSites { get; }
        IItemRepository Items { get; }
        IDetailRecordRepository DetailRecords { get; }
        IMovementRepository Movements { get; }
        ILocationRepository Locations { get; }
        IItemSupplyRepository ItemSupplies { get; }
        IItemSiteRepository ItemSites { get; }
        IRecordRepository Records { get; }
        IAttachmentAssignRepository AttachmentAssigns { get; }
        IAttachmentRepository Attachments { get; }
        IDirectorRepository Directors { get; }
        IFacultyRepository Faculties { get; }
        ILocationRecordRepository LocationRecords { get; }
        IUserVerificationRepository UserVerifications { get; }
        IDirectorWorkRepository DirectorWorks { get; }
        ILocationDirectorRepository LocationDirectors { get; }
        IArticleRepository Articles { get; }


    int Complete();
    }
}