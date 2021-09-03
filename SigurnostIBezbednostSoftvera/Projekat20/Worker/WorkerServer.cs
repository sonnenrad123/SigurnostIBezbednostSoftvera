using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Worker.Properties;

namespace Worker
{
    public class WorkerServer : ILBtoWorker
    {
        //Svi procesi ce sinhronizovati pristup txt fajlu preko EventWaitHandle
        static EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "SHARED_BY_ALL_PROCESSES");

        public List<string> DoSomeWork(List<string> input)
        {
            Console.WriteLine("Work accepted. Action: "+input[0]);
            if(input[0] == "GetElectricityConsumption")
            {
                return GetElectricityConsumption(input);
            }
            else if(input[0] == "AddEntity")
            {
                return AddEntity(input[1],input[2],input[3]);
            }
            else if(input[0] == "ModifyValue")
            {
                return ModifyValue(input[1], input[2]);
            }
            else if(input[0] == "ModifyID")
            {
                return ModifyID(input[1], input[2]);
            }
            else if(input[0] == "DeleteEntity")
            {
                return DeleteEntity(input[1]);
            }
            else if(input[0] == "DeleteDatabase")
            {
                return DeleteDatabase();
            }
            else if(input[0] == "ArchiveDatabase")
            {
                return ArchiveDatabase();
            }
            return new List<string>();
        }


        #region Logic for work with entities

        static List<string> GetElectricityConsumption(List<string> input)
        {
            //TODO add multiplication by consumer zone
            string imeprezime = input[1];
            int uid = -1;
            Int32.TryParse(input[2], out uid);

            List<string> database = ReadFromFile();

            foreach(string row in database)
            {
                string dbid, dbns, dbcons;
                GetValuesFromRow(row,out dbid, out dbns, out dbcons);
                if(dbid == input[2] && dbns == input[1])
                {
                    double endprice = 0;
                    double cons = 0;
                    Double.TryParse(dbcons,out cons);
                    ResourceSet rs = ConsumptionZones.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                    List<string> zones_and_prices = new List<string>();
                    foreach (DictionaryEntry entry in rs)
                    {
                        string resourceKey = entry.Key.ToString();
                        string resource = entry.Value.ToString();
                        string[] min_max_price = resource.Split(',');
                        double min, max, price;
                        if (min_max_price[1] == "inf")
                        {
                            max = Double.MaxValue;
                        }
                        else
                        {
                            Double.TryParse(min_max_price[1], out max);
                        }
                        Double.TryParse(min_max_price[0],out min);
                        
                        Double.TryParse(min_max_price[2], out price);
                        if(cons>=min && cons <= max)
                        {
                            endprice = cons * price;
                            return new List<string> {"Consumption info for ID:"+ dbid+", name and surname: "+dbns+" is:\n"+"\t\tZone: "+resourceKey+", Consumption: "+cons+", Price: "+endprice+"." };
                        }

                    }


                    
                }
            }
            return new List<string> { "Error", "User not found in database" };

        }

        static List<string> AddEntity(string id,string value,string name)
        {
            List<string> database = ReadFromFile();

            foreach(string row in database)
            {
                string dbid, dbns, dbcons;
                GetValuesFromRow(row, out dbid, out dbns, out dbcons);
                if(dbid == id)
                {
                    return new List<string> { "Error", "User already exists in database." };
                }
            }

            database.Add(id + "\t" + name + "\t" + value);
            WriteToFile(database);
            return new List<string> { "OK", "User added." };

        }

        static List<string> ModifyValue(string id, string newValue)
        {
            List<string> database = ReadFromFile();
            int cnt = 0;
            foreach (string row in database)
            {
                string dbid, dbns, dbcons;
                GetValuesFromRow(row, out dbid, out dbns, out dbcons);
                if (dbid == id)
                {
                    database[cnt] = dbid + "\t" + dbns + "\t" + newValue;
                    WriteToFile(database);
                    return new List<string> { "OK", "Value modified. Old consumption: "+dbcons+". New consumption: "+newValue+"." };
                }
                cnt++;
            }
            return new List<string> { "Error", "User doesn't exists in database." };
        }

        static List<string> ModifyID(string oldId,string newId)
        {
            List<string> database = ReadFromFile();
            int cnt = 0;

            foreach(string row in database)
            {
                string dbid, dbns, dbcons;
                GetValuesFromRow(row, out dbid, out dbns, out dbcons);
                if(dbid == newId)
                {
                    return new List<string> { "Error", "There is already ID in database same as new ID." };
                }
            }

            foreach (string row in database)
            {
                string dbid, dbns, dbcons;
                GetValuesFromRow(row, out dbid, out dbns, out dbcons);
                if (dbid == oldId)
                {
                    database[cnt] = newId + "\t" + dbns + "\t" + dbcons;
                    WriteToFile(database);
                    return new List<string> { "OK", "ID modified. Old id: " + oldId + ". New ID: " + newId + "." };
                }
                cnt++;
            }
            return new List<string> { "Error", "Old ID doesn't exists in database." };
        }

        static List<string> DeleteEntity(string id)
        {
            List<string> database = ReadFromFile();
            int cnt = 0;
            bool found = false;
            foreach (string row in database)
            {
                string dbid, dbns, dbcons;
                GetValuesFromRow(row, out dbid, out dbns, out dbcons);
                if (id == dbid)
                {
                    found = true;
                    break;
                }
                cnt++;
            }

            if (found)
            {
                string row = database[cnt];
                string dbid, dbns, dbcons;
                GetValuesFromRow(row, out dbid, out dbns, out dbcons);
                database.RemoveAt(cnt);
                WriteToFile(database);
                return new List<string> { "OK", "Consumer with ID="+dbid+", name and surname: "+dbns+" deleted." };
            }
            else
            {
                return new List<string> { "Error", "ID doesn't exists in database." };
            }
        }

        public List<string> DeleteDatabase()
        {
            List<string> database = ReadFromFile();
            database.Clear();
            WriteToFile(database);
            return new List<string> { "OK", "Database deleted succesfully" };
        }

        public List<string> ArchiveDatabase()
        {
            List<string> database = ReadFromFile();
            string arcname = WriteArchive(database);
            database.Clear();
            WriteToFile(database);

            return new List<string> { "OK", "Database archived succesfully. Name of archive: "+arcname };

        }
        #endregion
        #region Working with txt database
        //metode za citanje i upis u fajl
        static List<string> ReadFromFile()
        {
            List<string> retList = new List<string>();
            if (waitHandle.WaitOne())
            {
                try
                {
                    StreamReader sr = new StreamReader("database.txt");
                    string row = "";
                    while ((row = sr.ReadLine()) != null)
                    {
                        retList.Add(row);
                    }
                    sr.Close();
                }
                finally
                {
                    waitHandle.Set();
                }
            }
            return retList;
        }

        static void WriteToFile(List<string> toBeWritten)
        {
            if (waitHandle.WaitOne())
            {
                try
                {
                    StreamWriter sw = new StreamWriter("database.txt");
                    foreach(string row in toBeWritten)
                    {
                        sw.WriteLine(row);
                    }
                    sw.Close();
                }
                finally
                {
                    
                    waitHandle.Set();
                }
            }
        }

        static string WriteArchive(List<string> archivedDB)
        {
            string arcname = DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt";
            try
            {
                
                StreamWriter sw = new StreamWriter(arcname);
                foreach (string row in archivedDB)
                {
                    sw.WriteLine(row);
                }
                sw.Close();
            }
            finally
            {

                
            }
            return arcname;

        }

        static void GetValuesFromRow(string row,out string id,out string name_surname,out string consumption)
        {
            string[] values = row.Split('\t');
            id = values[0];
            name_surname = values[1];
            consumption = values[2];

        }
        #endregion
    }
}
