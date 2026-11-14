using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Data.Entities
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }
}
