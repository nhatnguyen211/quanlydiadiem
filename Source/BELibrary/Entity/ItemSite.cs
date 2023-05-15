namespace BELibrary.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ItemSite")]
    public partial class ItemSite
    {
        public Guid Id { get; set; }

        public Guid DetailItemSiteId { get; set; }

        public Guid DetailRecordId { get; set; }

        public virtual DetailItemSite DetailItemSite { get; set; }

        public virtual DetailRecord DetailRecord { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
    }
}