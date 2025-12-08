namespace HomeBudgetManager.Core;


// Simple register class containing creating new user and checking whether username is taken
public class RegisterService
{
    private HashPassword _hasher = new HashPassword();
    private string _path = Path.Combine(Directory.GetCurrentDirectory(), "registerData.txt");

    public bool isRegistered(string username)
    {
        if (File.Exists(_path))
        {
            using (StreamReader sr = new StreamReader(_path))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    
                    if (line.Contains(username))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    public void registerUser(string email, string username, string password)
    {
        File.AppendAllText(_path, email + ";" + username + ";" + _hasher.hash(password) + ";" + Environment.NewLine);
    }

}
