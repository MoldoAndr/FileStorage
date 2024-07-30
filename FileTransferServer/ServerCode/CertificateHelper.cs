using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public static class CertificateHelper
{
    public static X509Certificate2 GetOrCreateCertificate(string serverRootDirectory)
    {
        string certPath = Path.Combine(serverRootDirectory, "server.pfx");
        string certPassword = "YourSecurePassword";

        return File.Exists(certPath) ? new X509Certificate2(certPath, certPassword) : CreateSelfSignedCertificate(certPath, certPassword);
    }

    private static X509Certificate2 CreateSelfSignedCertificate(string certPath, string certPassword)
    {
        using (RSA rsa = RSA.Create(2048))
        {
            var request = new CertificateRequest("cn=FileTransferServer", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

            File.WriteAllBytes(certPath, certificate.Export(X509ContentType.Pfx, certPassword));

            return new X509Certificate2(certPath, certPassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
        }
    }
}
