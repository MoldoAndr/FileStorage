using System.ComponentModel.Design;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

public static class FileOperations
{
    public static void ReceiveFile(BinaryReader reader, string userFolder)
    {
        string fileName = reader.ReadString();
        long fileSize = reader.ReadInt64();
        string filePath = Path.Combine(userFolder, fileName);

        Console.WriteLine($"Receiving file {fileName} of size {fileSize} bytes.");

        try
        {
            DirectoryHelper.EnsureDirectoryExists(userFolder);

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                long totalBytesRead = 0;

                while (totalBytesRead < fileSize && (bytesRead = reader.Read(buffer, 0, (int)Math.Min(buffer.Length, fileSize - totalBytesRead))) > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                }
            }

            Console.WriteLine($"File {fileName} received successfully.");
        }
        catch (IOException ioEx)
        {
            Console.WriteLine($"IO Exception while receiving file: {ioEx.Message}");
        }
    }

    public static void SendFile(BinaryReader reader, BinaryWriter writer, string userFolder)
    {
        string fileName = reader.ReadString();
        string filePath = Path.Combine(userFolder, fileName);

        Console.WriteLine($"Sending file {fileName}.");

        try
        {
            if (File.Exists(filePath))
            {
                writer.Write(true);
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    writer.Write(fs.Length);
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
                Console.WriteLine($"File {fileName} sent successfully.");
            }
            else
            {
                writer.Write(false);
                Console.WriteLine($"File {fileName} not found.");
            }
        }
        catch (IOException ioEx)
        {
            Console.WriteLine($"IO Exception while sending file: {ioEx.Message}");
        }
    }

    public static void DeleteFile(BinaryReader reader, string userFolder)
    {
        string fileName = reader.ReadString();
        string filePath = Path.Combine(userFolder, fileName);

        Console.WriteLine($"Deleting file {fileName}.");

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine($"File {fileName} deleted successfully.");
            }
            else
            {
                Console.WriteLine($"File {fileName} not found.");
            }
        }
        catch (IOException ioEx)
        {
            Console.WriteLine($"IO Exception while deleting file: {ioEx.Message}");
        }
    }

    public static void ListFiles(BinaryWriter writer, string userFolder)
    {
        try
        {
            DirectoryHelper.EnsureDirectoryExists(userFolder);

            string[] files = Directory.GetFiles(userFolder);
            writer.Write(files.Length);
            foreach (string file in files)
            {
                writer.Write(Path.GetFileName(file));
            }
            Console.WriteLine("File list sent.");
        }
        catch (IOException ioEx)
        {
            Console.WriteLine($"IO Exception while listing files: {ioEx.Message}");
            writer.Write(0);
        }
    }

    public static void ShareFile(BinaryReader reader, BinaryWriter writer, string username)
    {
        string fileName = reader.ReadString();
        string recipientUsername = reader.ReadString();

        Console.WriteLine($"Sharing file {fileName} from {username} to {recipientUsername}.");

        try
        {
            string senderFolder = UserManager.GetUserFolder(username);
            string recipientFolder = UserManager.GetUserFolder(recipientUsername);

            string senderFilePath = Path.Combine(senderFolder, fileName);
            string recipientFilePath = Path.Combine(recipientFolder, fileName);

            if (!UserManager.UserExists(recipientUsername))
            {
                writer.Write(false);
                Console.WriteLine($"Recipient user {recipientUsername} does not exist.");
                return;
            }

            RetryFileOperation(() =>
            {
                if (File.Exists(senderFilePath))
                {
                    if (File.Exists(recipientFilePath))
                    {
                        writer.Write(false);
                        Console.WriteLine($"File {fileName} already exists in {recipientUsername}'s folder.");
                    }
                    else
                    {
                        DirectoryHelper.EnsureDirectoryExists(recipientFolder);
                        File.Copy(senderFilePath, recipientFilePath, overwrite: true);
                        writer.Write(true);
                        Console.WriteLine($"File {fileName} shared successfully to {recipientUsername}.");
                    }
                }
                else
                {
                    writer.Write(false);
                    Console.WriteLine($"File {fileName} not found in {username}'s folder.");
                }
            });
        }
        catch (IOException ioEx)
        {
            writer.Write(false);
            Console.WriteLine($"IO Exception while sharing file: {ioEx.Message}");
        }
    }

    public static void FileSend(BinaryReader reader, BinaryWriter writer, string username)
    {
        string fileName = reader.ReadString();
        string recipientUsername = reader.ReadString();
        Console.WriteLine($"Sending file {fileName} from {username} to {recipientUsername}.");
        try
        {
            if (!UserManager.UserExists(recipientUsername))
            {
                writer.Write(false);
                Console.WriteLine($"Recipient user {recipientUsername} does not exist.");
                return;
            }

            string senderFolder = UserManager.GetUserFolder(username);
            string recipientFolder = UserManager.GetUserFolder(recipientUsername);
            string senderFilePath = Path.Combine(senderFolder, fileName);
            string recipientFilePath = Path.Combine(recipientFolder, fileName);

            RetryFileOperation(() =>
            {
                if (File.Exists(senderFilePath))
                {
                    if (File.Exists(recipientFilePath))
                    {
                        writer.Write(false);
                        Console.WriteLine($"File {fileName} already exists in {recipientUsername}'s folder.");
                    }
                    else
                    {
                        DirectoryHelper.EnsureDirectoryExists(recipientFolder);
                        File.Copy(senderFilePath, recipientFilePath, overwrite: true);
                        File.Delete(senderFilePath);
                        writer.Write(true);
                        Console.WriteLine($"File {fileName} sent successfully to {recipientUsername} and deleted from {username}'s folder.");
                    }
                }
                else
                {
                    writer.Write(false);
                    Console.WriteLine($"File {fileName} not found in {username}'s folder.");
                }
            });
        }
        catch (IOException ioEx)
        {
            writer.Write(false);
            Console.WriteLine($"IO Exception while sending file: {ioEx.Message}");
        }
    }

    public static void RenameFile(BinaryReader reader, BinaryWriter writer, string username)
    {
        string folder = UserManager.GetUserFolder(username);
        string oldName = reader.ReadString();
        string newName = reader.ReadString();
        string oldPath = Path.Combine(folder, oldName);
        string newPath = Path.Combine(folder, newName);

        try
        {
            Console.WriteLine($"Attempting to rename file from {oldPath} to {newPath}");

                if (File.Exists(newPath))
                {
                    writer.Write(false);
                    Console.WriteLine($"Error: File {newPath} already exists.");
                }
                else
                {
                    File.Move(oldPath, newPath);
                    writer.Write(true);
                    Console.WriteLine($"File renamed successfully from {oldName} to {newName}.");
                }
        }
        catch (IOException ioEx)
        {
            writer.Write(false);
            Console.WriteLine($"IO Exception while renaming file: {ioEx.Message}");
        }
    }

    private static void RetryFileOperation(Action fileOperation, int maxRetries = 5, int delayBetweenRetries = 1000)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                fileOperation();
                return;
            }
            catch (IOException ioEx)
            {
                if (attempt == maxRetries)
                {
                    throw;
                }
                Console.WriteLine($"IO Exception: {ioEx.Message}. Retrying in {delayBetweenRetries}ms...");
                System.Threading.Thread.Sleep(delayBetweenRetries);
            }
        }
    }

    internal static void SendContent(BinaryReader reader, BinaryWriter writer, string userFolder)
    {
        string fileName = reader.ReadString();
        string filePath = Path.Combine(userFolder, fileName);

        Console.WriteLine($"Viewing file {fileName}.");
        if (File.Exists(filePath))
        {
            writer.Write(true);
            byte[] fileBytes = File.ReadAllBytes(filePath);
            writer.Write(fileBytes.Length);
            writer.Write(fileBytes);
            writer.Flush();

            Console.WriteLine($"Sent file {fileName} of size {fileBytes.Length} bytes.");
        }
        else
        {
            writer.Write(false);
        }
    }

}