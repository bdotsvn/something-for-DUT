using System;
using System.Linq;
using StudentManagementSystem.Interfaces;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public class FacultyService
    {
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly IRepository<Subject> _subjectRepository;
        private readonly IRepository<User> _userRepository;

        public FacultyService(
            IRepository<Enrollment> enrollmentRepository,
            IRepository<Subject> subjectRepository,
            IRepository<User> userRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _subjectRepository = subjectRepository;
            _userRepository = userRepository;
        }

        public void UpdateStudentGrade(int studentId, int subjectId, double score)
        {
            var enrollment = _enrollmentRepository.Query()
                .FirstOrDefault(e => e.StudentId == studentId && e.SubjectId == subjectId);

            if (enrollment != null)
            {
                // Since Enrollment is a class (reference type), updating property updates the object in memory (assuming shared reference in InMemoryRepository)
                // If it was a struct or record (and we wanted immutability), we'd need to replace it.
                // Our InMemoryRepository implementation stores the reference, so this works.
                enrollment.Score = score;
                Console.WriteLine("Grade updated successfully.");
            }
            else
            {
                Console.WriteLine("Enrollment not found.");
            }
        }

        public void ListStudentsInSubject(int subjectId)
        {
            var enrollments = _enrollmentRepository.Query().Where(e => e.SubjectId == subjectId).ToList();
            if (!enrollments.Any())
            {
                Console.WriteLine("No students enrolled in this subject.");
                return;
            }

            Console.WriteLine($"Students in Subject ID {subjectId}:");
            foreach (var enrollment in enrollments)
            {
                var student = _userRepository.GetById(enrollment.StudentId) as Student;
                if (student != null)
                {
                     Console.WriteLine($"- ID: {student.Id}, Name: {student.FullName}, Grade: {enrollment.Score?.ToString() ?? "N/A"}");
                }
            }
        }
    }
}
