using FinanceTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[IgnoreAntiforgeryToken]
public class LoginModel : PageModel
{
    private readonly AuthService _auth;

    public LoginModel(AuthService auth)
    {
        _auth = auth;
    }

    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";

    public string Error { get; set; } = "";

    public IActionResult OnPost()
    {
        Console.WriteLine("=== LOGIN POST ===");
        Console.WriteLine($"Email    : {Email}");
        Console.WriteLine($"Password : {Password}");

        var result = _auth.Login(Email, Password);

        Console.WriteLine($"Success: {result.Success}");
        Console.WriteLine($"Message: {result.Message}");

        if (!result.Success)
        {
            Error = result.Message;
            return Page();
        }

        HttpContext.Session.SetString("UserId", result.User!.Id.ToString());
        HttpContext.Session.SetString("Username", result.User.Username);

        return Redirect("/Dashboard");
    }
}
