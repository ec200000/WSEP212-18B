using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace WebApplication2
{
    public class MyCertificateValidationService
    {
        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            Console.WriteLine(Path.GetFullPath("server.crt"));
            var cert = new X509Certificate2(Path.GetFullPath("server.crt"), "sadna");
            if (clientCertificate.Thumbprint == cert.Thumbprint)
            {
                return true;
            }

            return false;
        }
    }
}