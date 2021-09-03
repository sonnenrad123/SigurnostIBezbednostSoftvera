using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface ISpecialUsers
    {
        [OperationContract]
        string ModifyValue(string id, string newValue); // only for operators

        [OperationContract]
        string ModifyID(string oldId, string newId);    // only for operators

        [OperationContract]
        string AddEntity(string Id, string value, string name);    // only for administrators

        [OperationContract]
        string DeleteEntity(string Id);    // only for administrators

        [OperationContract]
        string DeleteDatabase();    // only for super administrators

        [OperationContract]
        string GetElectricityConsumption(string imeprezime, string uid);

        [OperationContract]
        string GetSecretKey();
        [OperationContract]
        string ArchiveDatabase();
    }
}
