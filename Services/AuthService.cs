using System.Text.Json;
using FinanceTracker.Models;

namespace FinanceTracker.Services
{
    public class AuthService
    {
        private readonly string _filePath;

        public AuthService(IWebHostEnvironment env)
        {
            // Always use Data folder for JSON storage
            _filePath = Path.Combine(env.ContentRootPath, "Data", "users.json");

            var directory = Path.GetDirectoryName(_filePath)!;

            // Ensure Data folder exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Ensure users.json exists
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        // ================= READ USERS =================
        private List<User> GetAllUsers()
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        // ================= SAVE USERS =================
        private void SaveAllUsers(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }

        // ================= REGISTER =================
        public (bool Success, string Message) Register(User user)
        {
            var users = GetAllUsers();

            // Check if email already exists

            if (users.Any(u =>
                u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "User already registered. Please login.");
            }

            if (users.Any(u =>
                u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Username already exists.");
            }

            users.Add(user);
            SaveAllUsers(users);

            return (true, "Registration successful.");
        }

        // ================= LOGIN =================
        public (bool Success, string Message, User? User) Login(string email, string password)
        {
            email = email.Trim();
            password = password.Trim();

            var users = GetAllUsers();

            var user = users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (user == null)
            {
                return (false, "Invalid email or password.", null);
            }

            return (true, "Login successful.", user);
        }

        public bool UsernameExists(string username)
        {
            var users = GetAllUsers();
            return users.Any(u => u.Username == username);
        }

        public bool EmailExists(string email)
        {
            var users = GetAllUsers();
            return users.Any(u => u.Email == email);
        }
    }
}
