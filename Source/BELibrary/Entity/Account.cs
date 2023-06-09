namespace BELibrary.Entity
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Account")]
    public partial class Account
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(15)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(250)]
        public string LinkAvatar { get; set; }

        public bool Gender { get; set; }

        [StringLength(250)]
        public string Password { get; set; }

        public int Role { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? LocationId { get; set; }

        public virtual Location Location { get; set; }
        public Guid? DirectorId { get; set; }

        public virtual Director Director { get; set; }
    }
}