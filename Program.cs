using FinanceTracker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
// builder.Services.AddSession(options =>
// {
//     options.IdleTimeout = TimeSpan.FromMinutes(30);
// });
builder.Services.AddSession();

builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<FinanceService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapRazorPages();

app.Run();
