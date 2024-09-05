using System;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

public static class UserManager
{
    private static readonly string ConnectionString = "Server=tcp:SPIRIDUSUL,1433;Database=FileTransferDB;User Id=TransferDB;Password=parolaServer123;TrustServerCertificate=True;";
    
    public static bool ValidateUser(string username, string password)
    {
        string hashedPassword = HashPassword(password);

        try
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND HashedPassword = @HashedPassword";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@HashedPassword", hashedPassword);

                    int userCount = (int)cmd.ExecuteScalar();

                    return userCount > 0;
                }
            }
        }
        catch (SqlException sqlEx)
        {
            Console.WriteLine($"SQL Exception while validating user: {sqlEx.Message}");
            return false;
        }
    }

    public static bool CreateUser(string username, string password)
    {
        string hashedPassword = HashPassword(password);

        try
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string checkUserQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    int userExists = (int)checkCmd.ExecuteScalar();
                    if (userExists > 0)
                    {
                        return false;
                    }
                }

                // Insert new user
                string insertQuery = "INSERT INTO Users (Username, HashedPassword) VALUES (@Username, @HashedPassword)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@Username", username);
                    insertCmd.Parameters.AddWithValue("@HashedPassword", hashedPassword);

                    insertCmd.ExecuteNonQuery();
                }

                // Create user folder logic (if needed)
                string userFolder = GetUserFolder(username);
                DirectoryHelper.EnsureDirectoryExists(userFolder);

                Console.WriteLine($"User {username} created.");
                return true;
            }
        }
        catch (SqlException sqlEx)
        {
            Console.WriteLine($"SQL Exception while creating user: {sqlEx.Message}");
            return false;
        }
    }

    public static bool UserExists(string username)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    int userExists = (int)cmd.ExecuteScalar();

                    return userExists > 0;
                }
            }
        }
        catch (SqlException sqlEx)
        {
            Console.WriteLine($"SQL Exception while checking if user exists: {sqlEx.Message}");
            return false;
        }
    }

    public static byte[] GetUserEncryptionKey(string username)
    {
        string hashedPassword = null;

        try
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT HashedPassword FROM Users WHERE Username = @Username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        hashedPassword = reader.GetString(0);
                    }
                }
            }

            if (hashedPassword == null)
            {
                throw new Exception($"User '{username}' not found in the database.");
            }

            string combinedInput = username + hashedPassword;
            byte[] key = HashToKey(combinedInput);

            return key;
        }
        catch (SqlException sqlEx)
        {
            Console.WriteLine($"SQL Exception while getting user encryption key: {sqlEx.Message}");
            throw;
        }
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

    private static byte[] HashToKey(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return hash.Take(32).ToArray();
        }
    }

    public static string GetUserFolder(string username)
    {
        string hashedUsername = HashPassword(username);
        return Path.Combine(FileTransferServer.ServerRootDirectory, "Files", hashedUsername.Substring(0, 16));
    }
}
