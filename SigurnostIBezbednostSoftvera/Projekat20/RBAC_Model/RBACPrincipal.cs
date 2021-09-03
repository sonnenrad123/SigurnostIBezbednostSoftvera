using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace RBAC_Model
{
    public class RBACPrincipal : IPrincipal
    {
        WindowsIdentity identity = null;

        public RBACPrincipal(WindowsIdentity windowsIdentity) 
        {
            identity = windowsIdentity;
        }

        public IIdentity Identity
        {
            get { return identity; }
        }

        public bool IsInRole(string permission)
        {
            foreach (IdentityReference group in this.identity.Groups)//through all groups
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                var name = sid.Translate(typeof(NTAccount));
                string groupName = Formatter.ParseName(name.ToString());
                string[] permissions;
                if (RolesConfig.GetPermissions(groupName, out permissions))
                {
                   foreach (string permision in permissions) //through all permissions
                    {
                        if (permision.Equals(permission))//check permission match
                        {
                            try
                            {
                                Audit.AuthorizationSuccess(Formatter.ParseName(identity.Name),
                                OperationContext.Current.IncomingMessageHeaders.Action);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            return true;
                        }
                    }
                    // return permissions.Contains(permission);                   
                }
            }

            try
            {
               Audit.AuthorizationFailed(Formatter.ParseName(identity.Name),
               OperationContext.Current.IncomingMessageHeaders.Action, $"Need {permission} permission");
            }
            catch (Exception e)
            {
               Console.WriteLine(e.Message);
            }

            return false;
        }
    }
}
