using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class Program
    {
        static void Main(string[] args)
        {
            //host za glavnu serversku komponentu
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            string address = "net.tcp://localhost:9998/LoadBalancer";
            ServiceHost host = new ServiceHost(typeof(LoadBalancerServer));
            host.AddServiceEndpoint(typeof(ILoadBalancer), binding, address);
            host.Open();


            //host za worker role

            string lbCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);


            NetTcpBinding binding2 = new NetTcpBinding();

            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            string address2 = "net.tcp://localhost:9997/WorkerToLB";
            ServiceHost host2 = new ServiceHost(typeof(LoadBalancerWorkerServer));
            host2.AddServiceEndpoint(typeof(IWorkerToLB), binding2, address2);
            
            host2.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            
            
            host2.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host2.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, lbCertCN);

            try
            {
                host2.Open();
                Console.WriteLine("LoadBalancer started working. Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            finally
            {
                host2.Close();
            }



            
        }
    }
}
