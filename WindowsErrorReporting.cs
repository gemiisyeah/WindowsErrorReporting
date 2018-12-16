using Microsoft.Win32;
using System;
using System.Security.Principal;

namespace GemiisYeah.WER
{

    /// <summary>
    /// Windows Error Reporting Class gives way to enables or disable to you application
    /// 
    /// This needs to be run under administrator if not enabled first time, when enabled not admin will be required
    /// </summary>
    public class WindowsErrorReporting
    {
        /// <summary>
        /// As default Image name will use current application executable 
        /// </summary>
        public static String DefaultImageName
        {
            get => System.AppDomain.CurrentDomain.FriendlyName;
        }

        /// <summary>
        /// Enables Windows Error Reporting to you DefaultImageName
        /// 
        /// First time to enable, requires elevated process
        /// </summary>
        /// <param name="dumpFolder">Full path where save Dumps</param>
        /// <param name="dumpCount">Max dumps to save in path</param>
        /// <param name="dumpType">Default DumpType.Full</param>
        public static void Enable(string dumpFolder, int dumpCount = 10, DumpType dumpType = DumpType.Full)
        {
            Register(DefaultImageName, dumpFolder, dumpCount, dumpType);
        }

        /// <summary>
        /// Enables Windows Error Reporting to specific ImageFile
        /// 
        /// First time to enable, requires elevated process
        /// </summary>
        /// <param name="imageFile">Usefully to enable other ImageFile from you solution</param>
        /// <param name="dumpFolder">Full path where save Dumps</param>
        /// <param name="dumpCount">Max dumps to save in path</param>
        /// <param name="dumpType">Default DumpType.Full</param>
        public static void Install(string imageFile, string dumpFolder, int dumpCount = 10, DumpType dumpType = DumpType.Full)
        {
            Register(imageFile, dumpFolder, dumpCount, dumpType);
        }



        /// <summary>
        ///  Based on Guide => https://docs.microsoft.com/en-us/windows/desktop/wer/wer-settings
        ///  
        ///  We create next registry structure
        ///
        /// <!--WER Configuration for imageFile-->
        ///    <RegistryKey Root = "HKLM" Key="SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps\{imageFile}">
        ///      <RegistryValue Name = "DumpFolder" Type="string"/>
        ///      <RegistryValue Name = "DumpCount"  Type="integer"/>
        ///     <RegistryValue Name = "DumpType"   Type="integer"/>
        ///    </RegistryKey>
        ///    
        ///  Try to handle Os Arch, to write in right registry, like tipically 32-bit execution in 64-bit Os
        /// </summary>
        private static void Register(string imageFile, string dumpFolder, int dumpCount, DumpType dumpType)
        {
            using (RegistryKey dumpKey = GetKey($"SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting\\LocalDumps\\{imageFile}", RegistryHive.LocalMachine))
            {
                if (dumpKey == null)
                {
                    if (IsElevated)
                    {
                        using (RegistryKey key = GetKey($"SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting\\LocalDumps\\{imageFile}", RegistryHive.LocalMachine, true))
                        {
                            key.SetValue("DumpFolder", dumpFolder);
                            key.SetValue("DumpCount", dumpCount);
                            key.SetValue("DumpType", (int)dumpType);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disables Windows Error Reporting to specific ImageFile
        /// 
        /// this requires always elevated process, Next time you enable, requires elevated process
        /// </summary>
        /// <param name="imageFile"></param>
        public static void Disable(string imageFile)
        {
            using (RegistryKey registryKey = GetKey($"SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting\\LocalDumps\\{imageFile}", RegistryHive.LocalMachine))
            {
                if (registryKey != null)
                {
                    if (IsElevated)
                    {
                        using (RegistryKey key = GetKey($"SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting\\LocalDumps", RegistryHive.LocalMachine))
                            key.DeleteSubKeyTree(imageFile);
                    }
                }
            }
        }

        /// <summary>
        /// Disables Windows Error Reporting DefaultImageName
        /// 
        /// this requires always elevated process, Next time you enable, requires elevated process
        /// </summary>
        /// <param name="imageFile"></param>
        public static void Disable() => Disable(DefaultImageName);


        /// <summary>
        /// Here Does handle of Os Arch To get right Paths
        /// </summary>
        /// <returns></returns>
        private static RegistryKey GetKey(String keyPath, RegistryHive registryHive, bool create = false)
        {
            RegistryKey key;
            if (Environment.Is64BitOperatingSystem)
                key = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64);
            else
                key = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry32);

            if (create)
                key = key.CreateSubKey(keyPath);
            else
                key = key.OpenSubKey(keyPath, create);
            return key;
        }



        /// <summary>
        /// Check if run under elevated to prevent Not Permission Error
        /// </summary>
        static bool IsElevated
        {
            get
            {
                return WindowsIdentity.GetCurrent().Owner
                  .IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum DumpType : int
    {
        Custom = 0,
        Mini,
        Full
    }
}
