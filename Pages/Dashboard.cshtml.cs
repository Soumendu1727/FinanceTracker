using FinanceTracker.Models;
using FinanceTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[IgnoreAntiforgeryToken]
public class DashboardModel : PageModel
{
    private readonly FinanceService _finance;

    public DashboardModel(FinanceService finance)
    {
        _finance = finance;
    }

    // ===== UI DATA =====
    public string Username { get; set; } = "";
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Savings { get; set; }
    public decimal Investment { get; set; }

    public List<Transaction> Transactions { get; set; } = new();

    // ===== ADD FORM =====
    [BindProperty] public string Type { get; set; } = "";
    [BindProperty] public string Category { get; set; } = "";
    [BindProperty] public decimal Amount { get; set; }

    // ===== SEARCH =====
    [BindProperty(SupportsGet = true)] public string? SearchText { get; set; }
    [BindProperty(SupportsGet = true)] public string? SearchType { get; set; }
    [BindProperty(SupportsGet = true)] public decimal? SearchAmount { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? FromDate { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? ToDate { get; set; }

    // ===== LOAD PAGE =====
    public IActionResult OnGet()
    {
        if (!HttpContext.Session.Keys.Contains("UserId"))
            return Redirect("/Login");

        Username = HttpContext.Session.GetString("Username")!.ToUpper();
        var userId = Guid.Parse(HttpContext.Session.GetString("UserId")!);

        var data = _finance.GetTransactions(userId);

        // ===== SEARCH FILTERS =====
        if (!string.IsNullOrWhiteSpace(SearchText))
            data = data.Where(t =>
                t.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            ).ToList();

        if (!string.IsNullOrWhiteSpace(SearchType))
            data = data.Where(t => t.Type == SearchType).ToList();

        if (SearchAmount.HasValue)
            data = data.Where(t => t.Amount == SearchAmount.Value).ToList();

        if (FromDate.HasValue)
            data = data.Where(t => t.Date.Date >= FromDate.Value.Date).ToList();

        if (ToDate.HasValue)
            data = data.Where(t => t.Date.Date <= ToDate.Value.Date).ToList();

        Transactions = data;

        // ===== SUMMARY =====
        TotalIncome = data.Where(t => t.Type == "Income").Sum(t => t.Amount);
        TotalExpense = data.Where(t => t.Type == "Expense").Sum(t => t.Amount);
        Investment = data.Where(t => t.Type == "Investment").Sum(t => t.Amount);
        Savings = TotalIncome - TotalExpense;

        return Page();
    }

    // ===== ADD =====
    public IActionResult OnPost()
    {
        var userId = Guid.Parse(HttpContext.Session.GetString("UserId")!);

        _finance.SaveTransaction(new Transaction
        {
            UserId = userId,
            Type = Type,
            Category = Category,
            Amount = Amount,
            Date = DateTime.Now
        });

        return Redirect("/Dashboard");
    }

    // ===== UPDATE =====
    public IActionResult OnPostEdit(Guid id, string category, decimal amount)
    {
        var userId = Guid.Parse(HttpContext.Session.GetString("UserId")!);
        _finance.UpdateTransaction(userId, id, category, amount);
        return Redirect("/Dashboard");
    }

    // ===== DELETE =====
    public IActionResult OnPostDelete(Guid id)
    {
        var userId = Guid.Parse(HttpContext.Session.GetString("UserId")!);
        _finance.DeleteTransaction(userId, id);
        return Redirect("/Dashboard");
    }
}
