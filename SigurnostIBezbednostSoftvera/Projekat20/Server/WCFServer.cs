using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DecryptionAES;

namespace Server
{
    public class WCFServer : ISpecialUsers
    {
        public string GetElectricityConsumption(string imeprezime, string uid)
        {
            Console.WriteLine("Called method: GET ELECTRICITY CONSUMPTION");

            IIdentity identity = Thread.CurrentPrincipal.Identity;

            Console.WriteLine("Authentification type : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            Console.WriteLine("Client name : " + windowsIdentity.Name);

            Console.WriteLine("Encrypted messages: \n" + imeprezime + "\n" + uid);

            string decryptedImePrezime = DecryptionAlgorithm.DecryptMessage(imeprezime, SecretKey.sKey);
            string decryptedUId = DecryptionAlgorithm.DecryptMessage(uid, SecretKey.sKey);

            List<string> retValue = ForwardToLoadBalancer(new List<string> { "GetElectricityConsumption", decryptedImePrezime, decryptedUId});

            string retMessage="";

            foreach(string str in retValue)
            {
                retMessage = retMessage + '\t' + str;
            }
            return retMessage;
        }


        [PrincipalPermission(SecurityAction.Demand, Role = "Add")]
        public string AddEntity(string Id, string value,string name)
        {
            Console.WriteLine("Called method: ADD ENTITY");
            
            IIdentity identity = Thread.CurrentPrincipal.Identity;

            Console.WriteLine("Authentification type : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            Console.WriteLine("Client name : " + windowsIdentity.Name);

            Console.WriteLine("Encrypted messages: \n" + Id + "\n" + value + "\n" + name);

            string decryptedId = DecryptionAlgorithm.DecryptMessage(Id, SecretKey.sKey);
            string decryptedValue = DecryptionAlgorithm.DecryptMessage(value, SecretKey.sKey);
            string decryptedName = DecryptionAlgorithm.DecryptMessage(name, SecretKey.sKey);

            List<string> retValue = ForwardToLoadBalancer(new List<string> {"AddEntity",decryptedId, decryptedValue,decryptedName });

            string retMessage = "";

            foreach (string str in retValue)
            {
                retMessage = retMessage + '\t' + str;
            }
            return retMessage;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "DeleteAll")]
        public string DeleteDatabase()
        {
            Console.WriteLine("Called method: DELETE DATABASE");
            
            IIdentity identity = Thread.CurrentPrincipal.Identity;

            Console.WriteLine("Authentification type : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            Console.WriteLine("Client name : " + windowsIdentity.Name);

            List<string> retValue = ForwardToLoadBalancer(new List<string> {"DeleteDatabase" });

            string retMessage = "";

            foreach (string str in retValue)
            {
                retMessage = retMessage + '\t' + str;
            }
            return retMessage;
        }
        [PrincipalPermission(SecurityAction.Demand, Role = "DeleteAll")]
        public string ArchiveDatabase()
        {
            Console.WriteLine("Called method: ARCHIVE DATABASE");

            IIdentity identity = Thread.CurrentPrincipal.Identity;

            Console.WriteLine("Authentification type : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            Console.WriteLine("Client name : " + windowsIdentity.Name);

            List<string> retValue = ForwardToLoadBalancer(new List<string> { "ArchiveDatabase" });

            string retMessage = "";

            foreach (string str in retValue)
            {
                retMessage = retMessage + '\t' + str;
            }
            return retMessage;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Delete")]
        public string DeleteEntity(string Id)
        {
            Console.WriteLine("Called method: DELETE ENTITY");
            
            IIdentity identity = Thread.CurrentPrincipal.Identity;

            Console.WriteLine("Authentification type : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            Console.WriteLine("Client name : " + windowsIdentity.Name);

            Console.WriteLine("Encrypted messages: \n" + Id);

            string decryptedId = DecryptionAlgorithm.DecryptMessage(Id, SecretKey.sKey);

            List<string> retValue = ForwardToLoadBalancer(new List<string> {"DeleteEntity",decryptedId});

            string retMessage = "";

            foreach (string str in retValue)
            {
                retMessage = retMessage + '\t' + str;
            }
            return retMessage;
        }

       

        [PrincipalPermission(SecurityAction.Demand, Role = "Modify")]
        public string ModifyID(string oldId, string newId)
        {
            Console.WriteLine("Called method: MODIFY ID");
            
            IIdentity identity = Thread.CurrentPrincipal.Identity;

            Console.WriteLine("Authentification type : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            Console.WriteLine("Client name : " + windowsIdentity.Name);

            Console.WriteLine("Encrypted messages: \n" + oldId + "\n" + newId);

            string decryptedOldId = DecryptionAlgorithm.DecryptMessage(oldId, SecretKey.sKey);
            string decryptedNewId = DecryptionAlgorithm.DecryptMessage(newId, SecretKey.sKey);

            List<string> retValue = ForwardToLoadBalancer(new List<string> {"ModifyID", decryptedOldId, decryptedNewId });

            string retMessage = "";

            foreach (string str in retValue)
            {
                retMessage = retMessage + '\t' + str;
            }
            return retMessage;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Modify")]
        public string ModifyValue(string id, string newValue)
        {
            Console.WriteLine("Called method: MODIFY VALUE");
            
            IIdentity identity = Thread.CurrentPrincipal.Identity;

            Console.WriteLine("Authentification type : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            Console.WriteLine("Client name : " + windowsIdentity.Name);

            Console.WriteLine("Encrypted messages: \n" + id + "\n" + newValue);

            string decryptedId = DecryptionAlgorithm.DecryptMessage(id, SecretKey.sKey);
            string decryptedNewValue = DecryptionAlgorithm.DecryptMessage(newValue, SecretKey.sKey);

            List<string> retValue = ForwardToLoadBalancer(new List<string> {"ModifyValue", decryptedId, decryptedNewValue});

            string retMessage = "";

            foreach (string str in retValue)
            {
                retMessage = retMessage + '\t' + str;
            }
            return retMessage;
        }


        //funkcija za konektovanje na load balancer i prosledjivanje zahteva
        static List<string> ForwardToLoadBalancer(List<string> actionAndParameters)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            ChannelFactory<ILoadBalancer> factory = new ChannelFactory<ILoadBalancer>(binding, new
           EndpointAddress("net.tcp://localhost:9998/LoadBalancer"));
            ILoadBalancer proxy = factory.CreateChannel();
            List<string> returnValue = proxy.demandWork(actionAndParameters);
            return returnValue;
        }

        public string GetSecretKey()
        {
            return SecretKey.sKey;
        }

    }
    
}
