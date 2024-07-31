public static class CommandHandler
{
    public static void HandleAuthenticatedCommand(string command, BinaryReader reader, BinaryWriter writer, string username)
    {
        try
        {
            string userFolder = UserManager.GetUserFolder(username);
            switch (command)
            {
                case "UPLOAD":
                    FileOperations.ReceiveFile(reader, userFolder);
                    break;
                case "DOWNLOAD":
                    FileOperations.SendFile(reader, writer, userFolder);
                    break;
                case "DELETE":
                    FileOperations.DeleteFile(reader, userFolder);
                    break;
                case "LIST":
                    FileOperations.ListFiles(writer, userFolder);
                    break;
                case "SHARE":
                    FileOperations.ShareFile(reader, writer, username);
                    break;
                case "SEND":
                    FileOperations.FileSend(reader, writer, username);
                    break;
                default:
                    writer.Write("INVALID_COMMAND");
                    Console.WriteLine($"Invalid authenticated command received: {command}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling authenticated command {command}: {ex.Message}");
            writer.Write("COMMAND_ERROR");
        }
    }
}
