namespace HomeBudgetManager.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
public class HashPassword
{
    private readonly IPasswordHasher<string> _hasher; 
    // Prosta metoda weryfikująca użytkownika
    public HashPassword(){
        var options = new PasswordHasherOptions();
        
        var optionsWrapper = new OptionsWrapper<PasswordHasherOptions>(options);
        
        _hasher = new PasswordHasher<string>(optionsWrapper);
    }

    public string hash(string password){
        return _hasher.HashPassword("", password);
    }
    public bool verifyPassword(string hash1, string provided_password)
    {
        var result = _hasher.VerifyHashedPassword("", hash1, provided_password);
        if (result == PasswordVerificationResult.Success)
        {
            return true;
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            return true;
        }

        return false;

    }
}