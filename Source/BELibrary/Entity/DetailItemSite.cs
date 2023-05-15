namespace BELibrary.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DetailItemSite")]
    public partial class DetailItemSite
    {
        public Guid Id { get; set; }

        public int Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string Unit { get; set; }

        [Required]
        public string Note { get; set; }

        public Guid MovementId { get; set; }

        public virtual Movement Movement { get; set; }
    }
}