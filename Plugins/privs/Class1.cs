using System;
using System.Security.Principal;

namespace Plugin
{
    public class Plugin : MarshalByRefObject
    {
        public string Execute()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                return "Admin";
            }
            else if (principal.IsInRole(WindowsBuiltInRole.User))
            {
                return "User";
            }
            else
            {
                return "System";
            }
        }
    }
}
