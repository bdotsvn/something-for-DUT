using System;
using System.Linq;
using StudentManagementSystem.Interfaces;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public class StudentService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Enrollment> _enrollmentRepository;
        private readonly IRepository<Subject> _subjectRepository;

        public StudentService(
            IRepository<User> userRepository,
            IRepository<Enrollment> enrollmentRepository,
            IRepository<Subject> subjectRepository)
        {
            _userRepository = userRepository;
            _enrollmentRepository = enrollmentRepository;
            _subjectRepository = subjectRepository;
        }

        public void ViewResults(int studentId)
        {
            var enrollments = _enrollmentRepository.Query().Where(e => e.StudentId == studentId).ToList();

            Console.WriteLine("--- Academic Results ---");
            foreach(var enrollment in enrollments)
            {
                var subject = _subjectRepository.GetById(enrollment.SubjectId);
                if (subject != null)
                {
                    Console.WriteLine($"Subject: {subject.Name} ({subject.Code}) - Grade: {enrollment.Score?.ToString() ?? "N/A"}");
                }
            }
        }

        public void EditProfile(int studentId, string newEmail, string newFullName)
        {
            var student = _userRepository.GetById(studentId) as Student;
            if (student != null)
            {
                // Using record's `with` expression to create a copy with modified values
                var updatedStudent = student with { Email = newEmail, FullName = newFullName };
                _userRepository.Update(updatedStudent);
                Console.WriteLine("Profile updated successfully.");
            }
        }
    }
}
