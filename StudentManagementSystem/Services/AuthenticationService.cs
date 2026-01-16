using System.Linq;
using StudentManagementSystem.Interfaces;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<User> _userRepository;

        public AuthenticationService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public User? Login(string username, string password)
        {
            // Simple plain text comparison for this demo
            return _userRepository.Query()
                .FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public void ChangePassword(int userId, string newPassword)
        {
            var user = _userRepository.GetById(userId);
            if (user != null)
            {
                // Records are immutable by default for primary constructor properties if used that way,
                // but we can create a new record with updated values.
                // However, since we used `record User(...)` which generates init-only properties by default,
                // we should replace the object in the repository.

                // Pattern matching to create the specific type
                User updatedUser = user switch
                {
                    Admin admin => admin with { Password = newPassword },
                    Faculty faculty => faculty with { Password = newPassword },
                    Student student => student with { Password = newPassword },
                    _ => throw new InvalidOperationException("Unknown user type")
                };

                _userRepository.Update(updatedUser);
            }
        }
    }
}
