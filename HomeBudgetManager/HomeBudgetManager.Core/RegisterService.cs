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

    public bool IsEmailTaken(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }

    public void RegisterUser(string email, string username, string password)
    {
        var user = new DBUser
        {
            Email = email,
            Login = username,
            Password = _hasher.hash(password),
            Role = SystemRole.Guest
        };

        _context.Users.Add(user);
        _context.SaveChanges();
    }
}
