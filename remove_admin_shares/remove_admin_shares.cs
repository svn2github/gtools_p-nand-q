using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace remove_admin_shares
{
    class remove_admin_shares
    {
        static void Main(string[] args)
        {
            int sharesToDelete = 0;

            try
            {
                var searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM Win32_Share");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    ++sharesToDelete;
                    string shareName = queryObj["Name"].ToString();
                    Console.WriteLine(resource.IDS_DeleteingShare, shareName, queryObj["Path"]);
                    try
                    {
                        queryObj.Delete();
                    }
                    catch (ManagementException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.WriteLine(resource.IDS_ERR_UnableToDeleteShare, shareName);
                    }
                }
            }
            catch (ManagementException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(resource.IDS_ERR_UnableToQueryWMIData);
            }
            if (sharesToDelete != 0)
            {
                Console.WriteLine(resource.IDS_delete_summary, sharesToDelete);
            }
            else
            {
                Console.WriteLine(resource.IDS_no_share_to_delete);
            }
        }
    }
}
