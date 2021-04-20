using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace WebApplication2
{
    public class MyCertificateValidationService
    {
        public IConfiguration Configuration { get; set; }

        public MyCertificateValidationService(IConfiguration config)
        {
            Configuration = config;
        }
        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            var pass = Configuration["certificatePassword"];
            var cert = new X509Certificate2(Path.GetFullPath("server.crt"), "sadna");
            if (clientCertificate.Thumbprint == cert.Thumbprint)
            {
                return true;
            }

            return false;
        }
    }
}