using System.Text.RegularExpressions;
using FinanceTracker.Models;
using FinanceTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[IgnoreAntiforgeryToken]
public class RegisterModel : PageModel
{
    private readonly AuthService _auth;

    public RegisterModel(AuthService auth)
    {
        _auth = auth;
    }

    // ===== Bind Properties =====
    [BindProperty] public string Username { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";

    public string Error { get; set; } = "";

    public IActionResult OnPost()
    {
        // ---- Trim inputs ----
        Username = Username.Trim();
        Email = Email.Trim().ToLower();

        // ===== 1. Username validation =====
        var usernameRegex = new Regex("^[a-z0-9_]+$");

        if (string.IsNullOrWhiteSpace(Username))
        {
            Error = "Username is required";
            return Page();
        }

        if (!usernameRegex.IsMatch(Username))
        {
            Error = "Username can contain only lowercase letters, numbers, and _ (no spaces)";
            return Page();
        }

        // ===== 2. Check username exists =====
        if (_auth.UsernameExists(Username))
        {
            Error = "Username already registered";
            return Page();
        }

        // ===== 3. Check email exists =====
        if (_auth.EmailExists(Email))
        {
            Error = "Email already registered";
            return Page();
        }

        // ===== 4. Password validation =====
        if (Password.Length < 6)
        {
            Error = "Password must be at least 6 characters";
            return Page();
        }

        // ===== SAFE TO REGISTER =====
        var result = _auth.Register(new User
        {
            Username = Username,
            Email = Email,
            Password = Password
        });

        if (!result.Success)
        {
            Error = result.Message;
            return Page();
        }

        return Redirect("/Login");
    }
}
