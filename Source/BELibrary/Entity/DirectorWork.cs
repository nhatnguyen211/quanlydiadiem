using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BELibrary.Entity
{
    [Table("DirectorWork")]
    public class DirectorWork
    {
        public int Id { get; set; }

        public Guid DirectorId { get; set; }

        public Guid LocationId { get; set; }

        public DateTime ScheduleBook { get; set; }

        public int Status { get; set; }

        public virtual Director Director { get; set; }

        public virtual Location Location { get; set; }
    }
}