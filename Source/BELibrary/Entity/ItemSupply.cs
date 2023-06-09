namespace BELibrary.Entity
{
    using System;

    public partial class ItemSupply
    {
        public Guid Id { get; set; }

        public DateTime DateOfHire { get; set; }

        public int Status { get; set; }

        public int Amount { get; set; }

        public Guid ItemId { get; set; }

        public Guid LocationId { get; set; }

        public virtual Location Location { get; set; }

        public virtual Item Item { get; set; }
    }
}