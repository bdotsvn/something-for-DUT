using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StudentManagementSystem.Interfaces;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public class AdminService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Subject> _subjectRepository;
        private readonly IRepository<Enrollment> _enrollmentRepository;

        public AdminService(
            IRepository<User> userRepository,
            IRepository<Subject> subjectRepository,
            IRepository<Enrollment> enrollmentRepository)
        {
            _userRepository = userRepository;
            _subjectRepository = subjectRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        // Feature: Import students from CSV using PLINQ
        public void ImportStudentsFromCsv(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            var lines = File.ReadAllLines(filePath);

            // Skip header if exists, assume header is present
            var dataLines = lines.Skip(1);

            // Use PLINQ for parallel processing
            var students = dataLines.AsParallel().Select(line =>
            {
                var parts = line.Split(',');
                // CSV Format: Id,Username,Password,FullName,StudentCode,Email
                if (parts.Length >= 6)
                {
                    return new Student(
                        int.Parse(parts[0]),
                        parts[1],
                        parts[2],
                        parts[3],
                        parts[4]
                    ) { Email = parts[5] };
                }
                return null;
            }).Where(s => s != null).ToList();

            foreach (var student in students)
            {
                if (_userRepository.GetById(student!.Id) == null)
                {
                    _userRepository.Add(student);
                }
                else
                {
                     Console.WriteLine($"Student with ID {student.Id} already exists. Skipping.");
                }
            }

            Console.WriteLine($"Imported {students.Count} students.");
        }

        public void AddSubject(Subject subject)
        {
            if (_subjectRepository.GetById(subject.Id) == null)
            {
                _subjectRepository.Add(subject);
                Console.WriteLine("Subject added successfully.");
            }
            else
            {
                Console.WriteLine("Subject ID already exists.");
            }
        }

        public void AssignStudentToSubject(int studentId, int subjectId)
        {
            var student = _userRepository.GetById(studentId) as Student;
            var subject = _subjectRepository.GetById(subjectId);

            if (student == null || subject == null)
            {
                Console.WriteLine("Student or Subject not found.");
                return;
            }

            // Check if already enrolled
            var exists = _enrollmentRepository.Query().Any(e => e.StudentId == studentId && e.SubjectId == subjectId);
            if (!exists)
            {
                _enrollmentRepository.Add(new Enrollment(studentId, subjectId));
                Console.WriteLine($"Assigned {student.FullName} to {subject.Name}.");
            }
            else
            {
                Console.WriteLine("Student is already assigned to this subject.");
            }
        }

        public void ListAllSubjects()
        {
            var subjects = _subjectRepository.GetAll();
            foreach (var s in subjects)
            {
                Console.WriteLine($"ID: {s.Id}, Code: {s.Code}, Name: {s.Name}, Credits: {s.Credits}");
            }
        }
    }
}
