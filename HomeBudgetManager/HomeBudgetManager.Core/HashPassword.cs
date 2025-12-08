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
}