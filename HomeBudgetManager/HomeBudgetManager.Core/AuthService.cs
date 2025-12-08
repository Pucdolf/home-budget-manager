using System.IO;

namespace HomeBudgetManager.Core;

public class AuthService
{
    private HashPassword _hasher = new HashPassword();
    // Prosta metoda weryfikująca użytkownika
    private string _path = Path.Combine(Directory.GetCurrentDirectory(), "registerData.txt");

    public bool ValidateUser(string username, string password)
    {
        // W przyszłości tutaj podłączysz bazę danych
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        // Hardcoded admin na potrzeby przykładu
        if (username == "admin" && password == "1234") {
            return true;
        }

        using (StreamReader sr = new StreamReader(_path))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] fragments = line.Split(';');
                if (fragments[1] == username &&
                    _hasher.verifyPassword(fragments[2], password))
                {
                    return true;
                }
            }
        }
        return false;

    }

    // Metoda generująca powitanie
    public string GetWelcomeMessage(string username)
    {
        return $"Witaj w HomeBudgetManager, {username}! Twoje finanse są pod kontrolą.";
    }
}