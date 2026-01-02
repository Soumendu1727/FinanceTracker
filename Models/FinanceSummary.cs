namespace FinanceTracker.Models;

public class FinanceSummary
{
    public Guid UserId { get; set; }
    public decimal ManualIncome { get; set; }
    public decimal Investment { get; set; }
}
