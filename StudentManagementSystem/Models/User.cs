using System;

namespace StudentManagementSystem.Models
{
    public enum Role
    {
        Admin,
        Faculty,
        Student
    }

    public abstract record User(int Id, string Username, string Password, string FullName, Role Role);

    public record Admin(int Id, string Username, string Password, string FullName)
        : User(Id, Username, Password, FullName, Role.Admin);

    public record Faculty(int Id, string Username, string Password, string FullName)
        : User(Id, Username, Password, FullName, Role.Faculty);

    public record Student(int Id, string Username, string Password, string FullName, string StudentCode)
        : User(Id, Username, Password, FullName, Role.Student)
    {
        // Mutable property for editing profile, though records are typically immutable.
        // We can use init-only for ID/Username but allow updating others via standard properties if needed,
        // or just replace the record. For simplicity in this demo, we'll keep it immutable and replace on update,
        // or add a mutable property if strictly needed by "Edit Profile" feature without replacing the object identity.
        // Let's make Email mutable for the "Edit Profile" feature example.
        public string Email { get; set; } = "";
    }
}
