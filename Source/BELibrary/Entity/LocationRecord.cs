using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BELibrary.Entity
{
    [Table("LocationRecord")]
    public class LocationRecord
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime TestDate { get; set; }
        public double test { get; set; }
        public double test2 { get; set; }
        public double test3 { get; set; }
        public double test4 { get; set; }
        public double test5 { get; set; }
        public string Event { get; set; }
        public string Event2 { get; set; }
        public string ForManage2 { get; set; }
        public string ForManage { get; set; }
        public string ForDate2 { get; set; }
        public string ForDate { get; set; }
        public string EyePressureRight { get; set; }
        public string EyePressureLeft { get; set; }
        public string Formation { get; set; }
        public string Activity { get; set; }
        public string Activity2 { get; set; }
        public string Activity3 { get; set; }
        public string ImageProfile { get; set; }
        public string ImageProfile2 { get; set; }
        public string ImageProfile3 { get; set; }
        public bool Status { get; set; }
        public bool IsDelete { get; set; }
        public Guid? DirectorId { get; set; }
        public Guid RecordId { get; set; }
        public Guid LocationId { get; set; }
        public virtual Record Record { get; set; }
        public virtual Director Director { get; set; }
        public virtual Location Location { get; set; }
    }
}