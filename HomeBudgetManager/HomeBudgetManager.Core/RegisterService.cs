namespace HomeBudgetManager.Core;



// Simple register class containing creating new user and checking whether username is taken
public class RegisterService
{

    private string path = Path.Combine(Directory.GetCurrentDirectory(), "registerData.txt");

    public bool isRegistered(string username)
    {
        if (File.Exists(path))
        {
            using (StreamReader sr = new StreamReader(path))
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
        File.AppendAllText(path, email + ";" + username + ";" + password + ";" + Environment.NewLine);
    }

}
