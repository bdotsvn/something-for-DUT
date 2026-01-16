using System;
using System.Collections.Generic;

namespace StudentManagementSystem.Models
{
    public record Subject(int Id, string Code, string Name, int Credits);

    public class Enrollment
    {
        public int StudentId { get; init; }
        public int SubjectId { get; init; }
        public double? Score { get; set; } // Nullable, as they might not be graded yet.

        public Enrollment(int studentId, int subjectId)
        {
            StudentId = studentId;
            SubjectId = subjectId;
        }
    }
}
