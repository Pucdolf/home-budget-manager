namespace HomeBudgetManager.Core;

public class AuthService
{
    // Prosta metoda weryfikująca użytkownika
    public bool ValidateUser(string username, string password)
    {
        // W przyszłości tutaj podłączysz bazę danych
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        // Hardcoded admin na potrzeby przykładu
        return username == "admin" && password == "1234";
    }

    // Metoda generująca powitanie
    public string GetWelcomeMessage(string username)
    {
        return $"Witaj w HomeBudgetManager, {username}! Twoje finanse są pod kontrolą.";
    }
}