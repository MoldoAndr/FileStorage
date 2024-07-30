using System.Security.Cryptography;
using System.Text;

public static class UserManager
{
    private static readonly string UsersFile = Path.Combine(FileTransferServer.ServerRootDirectory, "Users", "users.txt");
    private static readonly string UsersFolder = Path.Combine(FileTransferServer.ServerRootDirectory, "Files");
    public static bool ValidateUser(string username, string password)
    {
        string hashedPassword = HashPassword(password);
        try
        {
            string[] lines = File.ReadAllLines(UsersFile);
            Console.WriteLine($"Username: {username}");
            Console.WriteLine($"Hashed Password: {hashedPassword}");

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                if (parts[0] == username && parts[1] == hashedPassword)
                {
                    Console.WriteLine("match");
                    return true;
                }
            }
        }
        catch (IOException ioEx)
        {
            Console.WriteLine($"IO Exception while validating user: {ioEx.Message}");
        }
        return false;
    }

    public static bool CreateUser(string username, string password)
    {
        string hashedPassword = HashPassword(password);
        try
        {
            string[] lines = File.ReadAllLines(UsersFile);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts[0] == username)
                {
                    return false;
                }
            }

            using (StreamWriter sw = File.AppendText(UsersFile))
            {
                sw.WriteLine($"{username},{hashedPassword}");
            }

            string userFolder = GetUserFolder(username);
            DirectoryHelper.EnsureDirectoryExists(userFolder);

            Console.WriteLine($"User {username} created.");
            return true;
        }
        catch (UnauthorizedAccessException uEx)
        {
            Console.WriteLine($"Unauthorized access: {uEx.Message}");
            return false;
        }
        catch (IOException ioEx)
        {
            Console.WriteLine($"IO Exception while creating user: {ioEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while creating user: {ex.Message}");
            return false;
        }
    }

    public static string GetUserFolder(string username)
    {
        string hashedUsername = HashPassword(username);
        return Path.Combine(UsersFolder, hashedUsername.Substring(0, 16));
    }

    private static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
