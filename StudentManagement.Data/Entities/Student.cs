using System;
using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Data.Entities
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? CourseId { get; set; }

        public virtual Course Course { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
