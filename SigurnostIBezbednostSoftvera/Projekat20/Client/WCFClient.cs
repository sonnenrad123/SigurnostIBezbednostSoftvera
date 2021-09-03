using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using EncryptionAES;

namespace Client
{
    public class WCFClient: ChannelFactory<ISpecialUsers>, ISpecialUsers
    {
        ISpecialUsers factory;
        NetTcpBinding binding2 = new NetTcpBinding();
        string sKey = "";



        public WCFClient(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
            sKey = factory.GetSecretKey();
        }

        public string GetElectricityConsumption(string imeprezime, string uid)
        {
            try
            {
               return factory.GetElectricityConsumption(EncryptionAlgorithm.EncryptMesssage(imeprezime, sKey),
                                                  EncryptionAlgorithm.EncryptMesssage(uid, sKey));
            }
            catch(SecurityAccessDeniedException es){
                Console.WriteLine("There was a error completing action: 'GetElectricityConsumption'. Message: " + es.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return "";
        }

        public string ModifyValue(string id, string newValue)
        {

            try
            {
                return factory.ModifyValue(EncryptionAlgorithm.EncryptMesssage(id, sKey),
                                    EncryptionAlgorithm.EncryptMesssage(newValue, sKey));
            }
            catch (SecurityAccessDeniedException es)
            {
                Console.WriteLine("There was a error completing action: 'ModifyValue'. Message: " + es.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return "";
        }

        public string ModifyID(string oldId, string newId)
        {

            try
            {
                return factory.ModifyID(EncryptionAlgorithm.EncryptMesssage(oldId, sKey),
                                 EncryptionAlgorithm.EncryptMesssage(newId, sKey));
            }
            catch (SecurityAccessDeniedException es)
            {
                Console.WriteLine("There was a error completing action: 'ModifyID'. Message: " + es.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return "";
        }

        public string AddEntity(string Id, string value,string name)
        {
            try
            {
                return factory.AddEntity(EncryptionAlgorithm.EncryptMesssage(Id, sKey),
                                  EncryptionAlgorithm.EncryptMesssage(value, sKey),
                                  EncryptionAlgorithm.EncryptMesssage(name, sKey));
            }
            catch (SecurityAccessDeniedException es)
            {
                Console.WriteLine("There was a error completing action: 'AddEntity'. Message: " + es.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return "";
        }

        public string DeleteEntity(string Id)
        {
            try
            {
                return factory.DeleteEntity(EncryptionAlgorithm.EncryptMesssage(Id, sKey));
            }
            catch (SecurityAccessDeniedException es)
            {
                Console.WriteLine("There was a error completing action: 'DeleteEntity'. Message: " + es.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return "";
        }

        public string DeleteDatabase()
        {
            try
            {
                return factory.DeleteDatabase();
            }
            catch (SecurityAccessDeniedException es)
            {
                Console.WriteLine("There was a error completing action: 'DeleteDatabase'. Message: " + es.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return "";
        }

        public string GetSecretKey()
        {
            return factory.GetSecretKey();
        }

        public string ArchiveDatabase()
        {
            try
            {
                return factory.ArchiveDatabase();
            }
            catch (SecurityAccessDeniedException es)
            {
                Console.WriteLine("There was a error completing action: 'ArchiveDatabase'. Message: " + es.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            return "";
        }
    }
}
