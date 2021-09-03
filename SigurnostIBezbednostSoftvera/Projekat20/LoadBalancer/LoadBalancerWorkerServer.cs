using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Contracts;
namespace LoadBalancer
{
    public class LoadBalancerWorkerServer : IWorkerToLB
    {
        public static ConcurrentDictionary<int, ILBtoWorker> workers = new ConcurrentDictionary<int, ILBtoWorker>();
        private int port = 9990;


        public int IamAliveGiveMeID()
        {
            return declareID();
        }

        public void RegisterMe(int id)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            int realport = port + id;
            ChannelFactory<ILBtoWorker> factory = new ChannelFactory<ILBtoWorker>(binding, new
           EndpointAddress("net.tcp://localhost:"+realport+"/LbToWorker"));
            ILBtoWorker proxy = factory.CreateChannel();
            workers.TryAdd(id, proxy);
            
        }

        public void UnRegisterMe(int id)
        {
            ILBtoWorker temp = null;
            workers.TryRemove(id, out temp);
        }


        private int declareID()
        {
            for(int i = 0; i < workers.Count; i++)
            {
                if (!workers.ContainsKey(i))
                {
                    return i;
                }
            }
            return workers.Count;
        }

       
    }
}
