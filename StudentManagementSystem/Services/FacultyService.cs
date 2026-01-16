using System;
using System.IO;
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

        public void ImportGradesFromCsv(string filePath)
        {
             if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            var dataLines = lines.Skip(1);

            // Using PLINQ to parse lines
            var gradeEntries = dataLines.AsParallel().Select(line =>
            {
                var parts = line.Split(',');
                // Format: StudentId,SubjectId,Score
                if (parts.Length >= 3 &&
                    int.TryParse(parts[0], out int sid) &&
                    int.TryParse(parts[1], out int subId) &&
                    double.TryParse(parts[2], out double score))
                {
                    return new { StudentId = sid, SubjectId = subId, Score = score };
                }
                return null;
            }).Where(x => x != null).ToList();

            int updatedCount = 0;
            foreach (var entry in gradeEntries)
            {
                var enrollment = _enrollmentRepository.Query()
                    .FirstOrDefault(e => e.StudentId == entry!.StudentId && e.SubjectId == entry.SubjectId);

                if (enrollment != null)
                {
                    enrollment.Score = entry.Score;
                    updatedCount++;
                }
                else
                {
                    // Optionally create enrollment if it doesn't exist?
                    // "Admin assigns student to subject" is the rule.
                    // But if Faculty has a grade list, maybe they should be able to ensure enrollment?
                    // Strict rule: Faculty only updates grades.
                    Console.WriteLine($"Enrollment not found for Student {entry!.StudentId} in Subject {entry.SubjectId}. Skipping.");
                }
            }
            Console.WriteLine($"Updated grades for {updatedCount} enrollments.");
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
