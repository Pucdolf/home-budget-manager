using HomeBudgetManager.Core.DBTables;

namespace HomeBudgetManager.Core;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly HashPassword _hasher = new();

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public bool ValidateUser(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.user_login == username);
        if (user == null) return false;

        return _hasher.verifyPassword(user.user_password, password);
    }

    public string GetWelcomeMessage(string username)
    {
        return $"Witaj w HomeBudgetManager, {username}! Twoje finanse są pod kontrolą.";
    }
}
