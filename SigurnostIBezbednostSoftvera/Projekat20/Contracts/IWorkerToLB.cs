using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IWorkerToLB
    {
        [OperationContract]
        int IamAliveGiveMeID();
        [OperationContract]
        void RegisterMe(int id);
        [OperationContract]
        void UnRegisterMe(int id);
    }
}
