using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class LoadBalancerServer : ILoadBalancer
    {
        static int lastworker = -1;
        public List<string> demandWork(List<string> input)
        {
            lastworker = (lastworker + 1) % (LoadBalancerWorkerServer.workers.Count);
            List<string> retList = new List<string>();
            string logline = "Work demand came from server. Action: " + input[0];
            if (input.Count > 1)
            {
                logline = logline + ". Parameters: ";
                for (int i = 1; i < input.Count; i++)
                {
                    if (i == input.Count - 1)
                    {
                        logline = logline + input[i] + ".";
                        break;
                    }
                    logline = logline + input[i] + ", ";
                }
            }
            if (LoadBalancerWorkerServer.workers.Count != 0)
            {

                int cnt = 0;
                
                foreach(ILBtoWorker item in LoadBalancerWorkerServer.workers.Values)
                {
                    if(cnt == lastworker)
                    {
                        return item.DoSomeWork(input);
                        //break;
                    }
                    cnt++;
                }
                
            }
            else
            {
                Console.WriteLine("No active workers!");
            }
            Console.WriteLine(logline);
            return retList;
        }
    }
}
