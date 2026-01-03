using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;

namespace HomeBudgetManager.Core;

public class RegisterService
{
    private readonly AppDbContext _context;
    private readonly HashPassword _hasher = new();

    public RegisterService(AppDbContext context)
    {
        _context = context;
    }

    public bool isRegistered(string username)
    {
        return _context.Users.Any(u => u.user_login == username);
    }

    public void registerUser(string email, string username, string password)
    {
        var user = new DBUser
        {
            user_email = email,
            user_login = username,
            user_password = _hasher.hash(password),
            user_role = SystemRole.Guest // lub inna domyślna rola, jeśli używasz enumów
        };

        _context.Users.Add(user);
        _context.SaveChanges();
    }
}
