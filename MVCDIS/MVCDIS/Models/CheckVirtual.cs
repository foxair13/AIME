using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Web;

namespace MVCDIS.Models
{
    public class CheckVirtual
    {
        public int isVirtualMachine()
        {
            const string microsoftcorporation = "microsoft corporation";
            const string vmware = "vmware";
            const string virtualbox = "virtualbox";
            foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                string manufacturer = item["Manufacturer"].ToString().ToLower();
                // Check the Manufacturer (eg: vmware, inc)
                if (manufacturer.Contains(microsoftcorporation) || manufacturer.Contains(vmware) || manufacturer.Contains(virtualbox))
                {
                    return 1;
                }

                // Also, check the model (eg: VMware Virtual Platform)
                if (item["Model"] != null)
                {
                    string model = item["Model"].ToString().ToLower();
                    if (model.Contains(microsoftcorporation) || model.Contains(vmware) || model.Contains(virtualbox))
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }
    }
}