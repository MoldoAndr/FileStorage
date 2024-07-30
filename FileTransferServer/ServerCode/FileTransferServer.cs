using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

class FileTransferServer
{
    private const int Port = 8888;
    public static readonly string ServerRootDirectory = @"C:\Users\Andrei\Desktop\FileTransferServer\FileTransferServer\";
    private static X509Certificate2 serverCertificate;

    static void Main(string[] args)
    {
        try
        {
            DirectoryHelper.EnsureDirectoryExists(ServerRootDirectory);

            serverCertificate = CertificateHelper.GetOrCreateCertificate(ServerRootDirectory);

            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ClientHandler.HandleClient(client, serverCertificate);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
