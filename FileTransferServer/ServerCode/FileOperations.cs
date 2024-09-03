using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class FileOperations
{

    public static void ReceiveFile(BinaryReader reader, string userFolder, string username)
    {
        string fileName = reader.ReadString();
        long fileSize = reader.ReadInt64();
        string filePath = Path.Combine(userFolder, fileName);

        Console.WriteLine($"Receiving file {fileName} of size {fileSize} bytes.");

        try
        {
            DirectoryHelper.EnsureDirectoryExists(userFolder);

            byte[] fileContent = reader.ReadBytes((int)fileSize);

            byte[] encryptionKey = UserManager.GetUserEncryptionKey(username);
            byte[] encryptedContent = EncryptFile(fileContent, encryptionKey);

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(encryptedContent, 0, encryptedContent.Length);
            }

            Console.WriteLine($"File {fileName} received and encrypted successfully.");
        }
        catch (IOException ioEx)
        {
            Console.WriteLine($"IO Exception while receiving file: {ioEx.Message}");
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

    public static void FileSend(BinaryReader reader, BinaryWriter writer, string username)
    {
        string fileName = reader.ReadString();
        string recipientUsername = reader.ReadString();

        Console.WriteLine($"Sending file {fileName} from {username} to {recipientUsername}.");

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

            if (File.Exists(senderFilePath))
            {
                byte[] fileContent = File.ReadAllBytes(senderFilePath);
                byte[] decryptedContent = DecryptFile(fileContent, UserManager.GetUserEncryptionKey(username));

                byte[] encryptedContent = EncryptFile(decryptedContent, UserManager.GetUserEncryptionKey(recipientUsername));

                if (File.Exists(recipientFilePath))
                {
                    recipientFilePath = GetUniqueFilePath(recipientFilePath);
                }

                DirectoryHelper.EnsureDirectoryExists(recipientFolder);
                File.WriteAllBytes(recipientFilePath, encryptedContent);
                File.Delete(senderFilePath);   

                writer.Write(true);
                Console.WriteLine($"File {fileName} sent successfully to {recipientUsername}.");
            }
            else
            {
                writer.Write(false);
                Console.WriteLine($"File {fileName} not found in {username}'s folder.");
            }
        }
        catch (IOException ioEx)
        {
            writer.Write(false);
            Console.WriteLine($"IO Exception while sharing file: {ioEx.Message}");
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

            if (File.Exists(senderFilePath))
            {
                byte[] fileContent = File.ReadAllBytes(senderFilePath);
                byte[] decryptedContent = DecryptFile(fileContent, UserManager.GetUserEncryptionKey(username));

                byte[] encryptedContent = EncryptFile(decryptedContent, UserManager.GetUserEncryptionKey(recipientUsername));

                if (File.Exists(recipientFilePath))
                {
                    recipientFilePath = GetUniqueFilePath(recipientFilePath);
                }

                DirectoryHelper.EnsureDirectoryExists(recipientFolder);
                File.WriteAllBytes(recipientFilePath, encryptedContent);

                writer.Write(true);
                Console.WriteLine($"File {fileName} shared successfully to {recipientUsername}.");
            }
            else
            {
                writer.Write(false);
                Console.WriteLine($"File {fileName} not found in {username}'s folder.");
            }
        }
        catch (IOException ioEx)
        {
            writer.Write(false);
            Console.WriteLine($"IO Exception while sharing file: {ioEx.Message}");
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

    public static void SendContent(BinaryReader reader, BinaryWriter writer, string userFolder, string username)
    {
        string fileName = reader.ReadString();
        string filePath = Path.Combine(userFolder, fileName);

        Console.WriteLine($"Viewing file {fileName}.");
        if (File.Exists(filePath))
        {
            writer.Write(true);

            byte[] encryptionKey = UserManager.GetUserEncryptionKey(username);

            byte[] encryptedFileBytes = File.ReadAllBytes(filePath);
            byte[] fileBytes = DecryptFile(encryptedFileBytes, encryptionKey);

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

    public static void SaveFile(BinaryReader reader, BinaryWriter writer, string username)
    {
        string fileName = reader.ReadString();
        int fileSize = reader.ReadInt32();
        string userFolder = UserManager.GetUserFolder(username);
        string filePath = Path.Combine(userFolder, fileName);

        Console.WriteLine($"Saving file {fileName} of size {fileSize} bytes for user {username}.");

        try
        {
            DirectoryHelper.EnsureDirectoryExists(userFolder);

            byte[] fileContent = reader.ReadBytes(fileSize);

            byte[] encryptedContent = EncryptFile(fileContent, UserManager.GetUserEncryptionKey(username));

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(encryptedContent, 0, encryptedContent.Length);
            }

            writer.Write(true);
            Console.WriteLine($"File {fileName} encrypted and saved successfully.");
        }
        catch (IOException ioEx)
        {
            writer.Write(false);
            Console.WriteLine($"IO Exception while saving file: {ioEx.Message}");
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

                byte[] fileContent = File.ReadAllBytes(filePath);

                byte[] encryptionKey = UserManager.GetUserEncryptionKey(Path.GetFileNameWithoutExtension(fileName));
                byte[] encryptedContent = EncryptFile(fileContent, encryptionKey);

                writer.Write(encryptedContent.Length);

                writer.Write(encryptedContent);

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

    private static byte[] EncryptFile(byte[] fileContent, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV();  

            using (MemoryStream ms = new MemoryStream())
            {
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);  
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(fileContent, 0, fileContent.Length);
                    }
                }
                return ms.ToArray();
            }
        }
    }

    private static byte[] DecryptFile(byte[] fileContent, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;

            using (MemoryStream ms = new MemoryStream(fileContent))
            {
                byte[] iv = new byte[aes.BlockSize / 8];
                ms.Read(iv, 0, iv.Length); 
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream decryptedStream = new MemoryStream())
                        {
                            cs.CopyTo(decryptedStream);
                            return decryptedStream.ToArray();
                        }
                    }
                }
            }
        }
    }

    private static string GetUniqueFilePath(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);

        int counter = 1;
        string newFilePath = filePath;

        while (File.Exists(newFilePath))
        {
            string tempFileName = $"{fileNameWithoutExtension}_{counter}{extension}";
            newFilePath = Path.Combine(directory, tempFileName);
            counter++;
        }

        return newFilePath;
    }
}