using StudentManagementSystem.Models;

namespace StudentManagementSystem.Interfaces
{
    public interface IAuthenticationService
    {
        User? Login(string username, string password);
        void ChangePassword(int userId, string newPassword);
    }
}
