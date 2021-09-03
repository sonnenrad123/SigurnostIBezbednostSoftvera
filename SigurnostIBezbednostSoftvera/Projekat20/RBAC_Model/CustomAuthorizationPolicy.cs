using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RBAC_Model
{
    public class CustomAuthorizationPolicy : IAuthorizationPolicy
    {
        public CustomAuthorizationPolicy()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }
        public string Id
        {
            get;
        }

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            //from context get identity
            if (!evaluationContext.Properties.TryGetValue("Identities", out object list))//out list of identity 
            {
                return false;
            }

            IList<IIdentity> identities = list as IList<IIdentity>;
            if (list == null || identities.Count <= 0)
            {
                return false;
            }

            bool success = false;

            try
            {
                Audit.AuthenticationSuccess(Formatter.ParseName(((WindowsIdentity)identities[0]).Name));
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (!success)
            {
                try
                {
                    Audit.AuthenticationFailed(Formatter.ParseName(((WindowsIdentity)identities[0]).Name));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            evaluationContext.Properties["Principal"] = new RBACPrincipal((WindowsIdentity)identities[0]);//get windowsidentity
            return true;
        }
    }
}
