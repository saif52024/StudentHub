using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentHub.Models
{
    [Table("marks")]
    public class Mark
    {
        [Key]
        [Column("MarkId")]
        public int MarkId { get; set; }

        [Required, StringLength(100)]
        [Column("SubjectName")]
        public string SubjectName { get; set; }

        [Column("MarksObtained")]
        public int MarksObtained { get; set; }

        [Column("TotalMarks")]
        public int TotalMarks { get; set; }

        [NotMapped]
        public double Percentage => TotalMarks > 0 ? (double)MarksObtained / TotalMarks * 100 : 0;

        [ForeignKey("Student")]
        [Column("StudentId")]
        public int StudentId { get; set; }

        public Student Student { get; set; }
    }
}
