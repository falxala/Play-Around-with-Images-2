using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayAroundwithImages2
{
    class SerchBlender
    {
        public static string[] GetUninstallList()
        {
            List<string> ret = new List<string>();

            string uninstall_path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            Microsoft.Win32.RegistryKey uninstall = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(uninstall_path, false);
            if (uninstall != null)
            {
                foreach (string subKey in uninstall.GetSubKeyNames())
                {
                    string InstallLocation = null;
                    Microsoft.Win32.RegistryKey appkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(uninstall_path + "\\" + subKey, false);
                    if (appkey.GetValue("DisplayName") != null)
                    {
                        if (appkey.GetValue("DisplayName").ToString().ToLower() == "blender")
                        {
                            InstallLocation = appkey.GetValue("InstallLocation").ToString();
                            ret.Add(InstallLocation);
                        }
                    }
                }
            }

            return ret.ToArray();
        }

    }
}
