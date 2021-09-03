using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Threading.Tasks;
using System.ServiceModel;

namespace RBAC_Model
{
    public class CustomAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            RBACPrincipal principal = operationContext.ServiceSecurityContext.
                 AuthorizationContext.Properties["Principal"] as RBACPrincipal;

            bool retVal = principal.IsInRole("Read");

            if (!retVal)
            {
                try
                {
                    Audit.AuthorizationFailed(Formatter.ParseName(principal.Identity.Name),
                        OperationContext.Current.IncomingMessageHeaders.Action, "Need Read permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return retVal;
        }
    }
}
