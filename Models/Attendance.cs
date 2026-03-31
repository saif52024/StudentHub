using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentHub.Models
{
    [Table("attendances")]   // ✅ Correct plural name
    public class Attendance
    {
        [Key]
        [Column("AttendanceId")]
        public int AttendanceId { get; set; }

        [Required]
        [Column("Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        [Column("Status")]
        public string Status { get; set; }

        [ForeignKey("Student")]
        [Column("StudentId")]
        public int StudentId { get; set; }

        public Student Student { get; set; }
    }
}
