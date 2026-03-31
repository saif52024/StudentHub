using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentHub.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }
        public string Gender { get; set; }
        public string Semester { get; set; }
        public string Contact { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }

        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }

}
