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

    public bool ValidateUserByEmail(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return false;

        return _hasher.verifyPassword(user.Password, password);
    }

    public string GetWelcomeMessage(string username)
    {
        return $"Witaj w HomeBudgetManager, {username}! Twoje finanse są pod kontrolą.";
    }

    public DBUser? GetUserByUsername(string username)
    {
        return _context.Users.FirstOrDefault(u => u.Login == username);
    }

    public DBUser? GetUserByEmail(string email)
    {
        return _context.Users.FirstOrDefault(u => u.Email == email);
    }
}
