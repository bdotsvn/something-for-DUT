using StudentManagementSystem.Models;
using StudentManagementSystem.Interfaces;

namespace StudentManagementSystem.Data
{
    public static class SeedData
    {
        public static void Initialize(IRepository<User> userRepository, IRepository<Subject> subjectRepository)
        {
            // Add default Admin
            if (!userRepository.Query().Any(u => u.Role == Role.Admin))
            {
                userRepository.Add(new Admin(1, "admin", "admin123", "System Administrator"));
            }

            // Add default Faculty
            if (!userRepository.Query().Any(u => u.Role == Role.Faculty))
            {
                userRepository.Add(new Faculty(2, "faculty", "faculty123", "Dr. Faculty Manager"));
            }

            // Add default Student
            if (!userRepository.Query().Any(u => u.Role == Role.Student))
            {
                userRepository.Add(new Student(3, "student", "student123", "Nguyen Van A", "SV001") { Email = "student@example.com" });
            }

            // Add default Subjects
            if (!subjectRepository.Query().Any())
            {
                subjectRepository.Add(new Subject(1, "M001", "Introduction to C#", 3));
                subjectRepository.Add(new Subject(2, "M002", "Advanced OOP", 4));
                subjectRepository.Add(new Subject(3, "M003", "Database Systems", 3));
            }
        }
    }
}
