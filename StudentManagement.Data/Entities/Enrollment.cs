using System;

namespace StudentManagement.Data.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public DateTime EnrolledOn { get; set; } = DateTime.Now;

        public virtual Student Student { get; set; }

        public virtual Course Course { get; set; }
    }
}
