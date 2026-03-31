using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentHub.Models
{
    public class Department
{
    [Key]
    public int DepartmentId { get; set; }

    [Required]
    public string DepartmentName { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
}
