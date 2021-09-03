using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            string wCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            string lbCertCN = "loadbalancer";
            int port = 9990;

            NetTcpBinding binding = new NetTcpBinding();

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            X509Certificate2 lbCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, lbCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9997/WorkerToLB"), new X509CertificateEndpointIdentity(lbCert));

            

            ChannelFactory<IWorkerToLB> factory = new ChannelFactory<IWorkerToLB>(binding, address);

            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            
            factory.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            factory.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, wCertCN);
            


            IWorkerToLB proxy = null;
            int id = -1;
            try
            {
                proxy = factory.CreateChannel();
                //javi se load balanceru i trazi svoj id
                id = proxy.IamAliveGiveMeID();

            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            

            

            
            //dobili id sad otvaramo konekciju
            NetTcpBinding binding2 = new NetTcpBinding();
            binding2.Security.Mode = SecurityMode.Transport;
            binding2.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            int realport = port + id;
            string address2 = "net.tcp://localhost:" + realport + "/LbToWorker";
            ServiceHost host = new ServiceHost(typeof(WorkerServer));
            host.AddServiceEndpoint(typeof(ILBtoWorker), binding2, address2);
            host.Open();
            proxy.RegisterMe(id);
            Console.WriteLine("Worker started working. Press anykey to stop.");
            Console.ReadKey();
            proxy.UnRegisterMe(id);
            host.Close();
        }
    }
}
