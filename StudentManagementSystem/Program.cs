using System;
using StudentManagementSystem.Data;
using StudentManagementSystem.Interfaces;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;

// Top-level statements (C# 9+)
Console.WriteLine("Initializing Student Management System...");

// Setup Dependency Injection (Manual for Console App simplicity)
var userRepository = new InMemoryRepository<User>(u => u.Id);
var subjectRepository = new InMemoryRepository<Subject>(s => s.Id);
var enrollmentRepository = new InMemoryRepository<Enrollment>(e => e.GetHashCode()); // Simple hashcode ID for demo

SeedData.Initialize(userRepository, subjectRepository);

var authService = new AuthenticationService(userRepository);
var adminService = new AdminService(userRepository, subjectRepository, enrollmentRepository);
var facultyService = new FacultyService(enrollmentRepository, subjectRepository, userRepository);
var studentService = new StudentService(userRepository, enrollmentRepository, subjectRepository);

User? currentUser = null;

while (true)
{
    if (currentUser == null)
    {
        Console.WriteLine("\n--- Login ---");
        Console.Write("Username: ");
        var username = Console.ReadLine();
        Console.Write("Password: ");
        var password = Console.ReadLine();

        currentUser = authService.Login(username ?? "", password ?? "");

        if (currentUser == null)
        {
            Console.WriteLine("Invalid credentials. Try again.");
        }
        else
        {
            Console.WriteLine($"Welcome, {currentUser.FullName} ({currentUser.Role})");
        }
    }
    else
    {
        ShowMenu(currentUser);
    }
}

void ShowMenu(User user)
{
    Console.WriteLine("\n--- Menu ---");
    switch (user.Role)
    {
        case Role.Admin:
            ShowAdminMenu();
            break;
        case Role.Faculty:
            ShowFacultyMenu();
            break;
        case Role.Student:
            ShowStudentMenu((Student)user);
            break;
    }
    Console.WriteLine("0. Logout");
    Console.WriteLine("X. Exit");

    Console.Write("Select option: ");
    var choice = Console.ReadLine();

    if (choice == "0")
    {
        currentUser = null;
        return;
    }
    if (choice?.ToUpper() == "X")
    {
        Environment.Exit(0);
    }

    HandleMenuSelection(user, choice);
}

void ShowAdminMenu()
{
    Console.WriteLine("1. Import Students from CSV");
    Console.WriteLine("2. Add Subject");
    Console.WriteLine("3. Assign Student to Subject");
    Console.WriteLine("4. List Subjects");
}

void ShowFacultyMenu()
{
    Console.WriteLine("1. List Students in Subject");
    Console.WriteLine("2. Update Student Grade");
    Console.WriteLine("3. Import Grades from CSV");
}

void ShowStudentMenu(Student student)
{
    Console.WriteLine("1. View Results");
    Console.WriteLine("2. Edit Profile");
}

void HandleMenuSelection(User user, string? choice)
{
    switch (user.Role)
    {
        case Role.Admin:
            HandleAdminChoice(choice);
            break;
        case Role.Faculty:
            HandleFacultyChoice(choice);
            break;
        case Role.Student:
            HandleStudentChoice((Student)user, choice);
            break;
    }
}

void HandleAdminChoice(string? choice)
{
    switch (choice)
    {
        case "1":
            Console.Write("Enter CSV path (format: Id,Username,Password,FullName,StudentCode,Email): ");
            var path = Console.ReadLine();
            if(path != null) adminService.ImportStudentsFromCsv(path);
            break;
        case "2":
            Console.Write("Enter ID: ");
            if(int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Enter Code: ");
                var code = Console.ReadLine();
                Console.Write("Enter Name: ");
                var name = Console.ReadLine();
                Console.Write("Enter Credits: ");
                if(int.TryParse(Console.ReadLine(), out int credits))
                {
                    adminService.AddSubject(new Subject(id, code ?? "", name ?? "", credits));
                }
            }
            break;
        case "3":
             Console.Write("Enter Student ID: ");
             int.TryParse(Console.ReadLine(), out int sid);
             Console.Write("Enter Subject ID: ");
             int.TryParse(Console.ReadLine(), out int subid);
             adminService.AssignStudentToSubject(sid, subid);
            break;
        case "4":
            adminService.ListAllSubjects();
            break;
        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}

void HandleFacultyChoice(string? choice)
{
    switch (choice)
    {
        case "1":
            Console.Write("Enter Subject ID: ");
            if (int.TryParse(Console.ReadLine(), out int subId))
            {
                facultyService.ListStudentsInSubject(subId);
            }
            break;
        case "2":
            Console.Write("Enter Student ID: ");
            int.TryParse(Console.ReadLine(), out int studId);
            Console.Write("Enter Subject ID: ");
            int.TryParse(Console.ReadLine(), out int subjId);
            Console.Write("Enter Score: ");
            if(double.TryParse(Console.ReadLine(), out double score))
            {
                facultyService.UpdateStudentGrade(studId, subjId, score);
            }
            break;
        case "3":
            Console.Write("Enter CSV path (format: StudentId,SubjectId,Score): ");
            var path = Console.ReadLine();
            if(path != null) facultyService.ImportGradesFromCsv(path);
            break;
        default:
             Console.WriteLine("Invalid option.");
            break;
    }
}

void HandleStudentChoice(Student student, string? choice)
{
    switch (choice)
    {
        case "1":
            studentService.ViewResults(student.Id);
            break;
        case "2":
            Console.Write("Enter new Full Name: ");
            var name = Console.ReadLine();
            Console.Write("Enter new Email: ");
            var email = Console.ReadLine();
            studentService.EditProfile(student.Id, email ?? student.Email, name ?? student.FullName);
            break;
        default:
             Console.WriteLine("Invalid option.");
            break;
    }
}
