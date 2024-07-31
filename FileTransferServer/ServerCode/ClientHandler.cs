using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

public static class ClientHandler
{
    public static void HandleClient(TcpClient client, X509Certificate2 serverCertificate)
    {
        Console.WriteLine("Client connected.");
        try
        {
            using (SslStream sslStream = new SslStream(client.GetStream(), false))
            {
                sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);

                using (BinaryReader reader = new BinaryReader(sslStream))
                using (BinaryWriter writer = new BinaryWriter(sslStream))
                {
                    string username = null;
                    bool authenticated = false;
                    while (true)
                    {
                        try
                        {
                            string command = reader.ReadString();
                            switch (command)
                            {
                                case "SIGNUP":
                                    HandleSignup(reader, writer);
                                    break;
                                case "LOGIN":
                                    username = HandleLogin(reader, writer, ref authenticated);
                                    break;
                                case "UPLOAD":
                                case "DOWNLOAD":
                                case "DELETE":
                                case "LIST":
                                case "SHARE":
                                case "SEND":
                                    if (authenticated)
                                    {
                                        CommandHandler.HandleAuthenticatedCommand(command, reader, writer, username);
                                    }
                                    else
                                    {
                                        writer.Write("NOT_AUTHENTICATED");
                                        Console.WriteLine("Unauthenticated access attempt.");
                                    }
                                    break;
                                case "DISCONNECT":
                                    Console.WriteLine("Client requested disconnect.");
                                    return;
                                default:
                                    writer.Write("INVALID_COMMAND");
                                    Console.WriteLine($"Invalid command received: {command}");
                                    break;
                            }
                        }
                        catch (IOException ioEx)
                        {
                            Console.WriteLine($"IO Exception: {ioEx.Message}");
                            break;
                        }
                    }
                }
            }
        }
        catch (AuthenticationException e)
        {
            Console.WriteLine($"Authentication failed - closing the connection. {e.Message}");
            client.Close();
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client disconnected.");
        }
    }

    private static void HandleSignup(BinaryReader reader, BinaryWriter writer)
    {
        string newUsername = reader.ReadString();
        string newPassword = reader.ReadString();
        bool userCreated = UserManager.CreateUser(newUsername, newPassword);
        writer.Write(userCreated);
        Console.WriteLine(userCreated ? $"New user created: {newUsername}" : $"Failed to create user: {newUsername}");
    }

    private static string HandleLogin(BinaryReader reader, BinaryWriter writer, ref bool authenticated)
    {
        string username = reader.ReadString();
        string password = reader.ReadString();
        authenticated = UserManager.ValidateUser(username, password);
        writer.Write(authenticated);
        Console.WriteLine(authenticated ? $"User authenticated: {username}" : $"Authentication failed for user: {username}");
        return authenticated ? username : null;
    }
}