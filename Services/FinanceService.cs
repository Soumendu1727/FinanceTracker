using System.Text.Json;
using FinanceTracker.Models;

namespace FinanceTracker.Services;

public class FinanceService
{
    private readonly string _filePath;

    public FinanceService(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", "transactions.json");

        var dir = Path.GetDirectoryName(_filePath)!;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");
    }

    // ---------- HELPERS ----------
    private List<Transaction> GetAll()
    {
        return JsonSerializer.Deserialize<List<Transaction>>(
            File.ReadAllText(_filePath)
        ) ?? new List<Transaction>();
    }

    private void SaveAll(List<Transaction> transactions)
    {
        File.WriteAllText(
            _filePath,
            JsonSerializer.Serialize(transactions, new JsonSerializerOptions
            {
                WriteIndented = true
            })
        );
    }

    // ---------- CRUD ----------
    public List<Transaction> GetTransactions(Guid userId)
    {
        return GetAll()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToList();
    }

    public void SaveTransaction(Transaction tx)
    {
        var all = GetAll();
        all.Add(tx);                // ✅ NEW transaction only
        SaveAll(all);
    }

    public void UpdateTransaction(Guid userId, Guid txId, string category, decimal amount)
    {
        var all = GetAll();

        var tx = all.FirstOrDefault(t =>
            t.UserId == userId &&
            t.Id == txId);

        if (tx != null)
        {
            tx.Category = category;
            tx.Amount = amount;
            // ❌ Id is NOT touched
        }

        SaveAll(all);
    }

    public void DeleteTransaction(Guid userId, Guid txId)
    {
        var all = GetAll();

        all.RemoveAll(t =>
            t.UserId == userId &&
            t.Id == txId);

        SaveAll(all);
    }
}
