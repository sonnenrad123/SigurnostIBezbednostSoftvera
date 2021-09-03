using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBAC_Model
{
    public class RolesConfig
    {
        //path to RolesConfigFile.resx
        static string path = @"";
        public static bool GetPermissions(string rolename, out string[] permissions)
        {
            
            permissions = new string[10];
            string permissionString = " ";

            permissionString = (string)RolesConfigFile.ResourceManager.GetObject(rolename);//get permission of this role
            if (permissionString != null)
            {
                permissions = permissionString.Split(','); // exemple p1,p2,p3
                return true;
            }
            return false;

        }

    }
}
