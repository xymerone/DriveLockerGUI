using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Microsoft.Win32;
using System.Windows.Forms;

namespace DriveLockerGUI
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static string COnfigKeyName = "DriveLocker"; 
        public static string sald = "AypiaCgw#N";
        public static RegistryKey options;
        [STAThread]
        
        static void Main()
        {
            options = GetRegistryOpt();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }
        public static RegistryKey GetRegistryOpt()
        {
            if (!ExistsConfigRegistry())
            {
                RegistryKey currentUserKey = Registry.CurrentUser
                .OpenSubKey(@"Software", true);
                RegistryKey DLKey = currentUserKey.CreateSubKey(COnfigKeyName);
                DLKey.SetValue("AutorunName", "DriveLockerGUI");
                DLKey.SetValue("Autorun", 0);
                DLKey.SetValue("Autostart", 0);
                DLKey.SetValue("HideStart", 0);
                DLKey.SetValue("TimeoutBlock", 3500);
                DLKey.SetValue("Hash", "");
                DLKey.SetValue("NameKeyFile", "DriveLocker.key");
                DLKey.Close();
                return Registry.CurrentUser
                    .OpenSubKey(@"Software\" + COnfigKeyName, true);
            }
            else
            {
                return Registry.CurrentUser
                    .OpenSubKey(@"Software\" + COnfigKeyName, true);
            }
            

        }
        static bool ExistsConfigRegistry()
        {
            RegistryKey currentUserKey = Registry.CurrentUser
                .OpenSubKey(@"Software");
            foreach(string s in currentUserKey.GetSubKeyNames())
            {
                if (s == COnfigKeyName)
                {
                    return true;
                }
            }
            return false;
        }
        
        public static List<string> GetFlashka()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<string> Flash = new List<string>(1);
            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType.ToString() != "Removable") // Только флешки
                {
                    continue;
                }
                Flash.Add(d.Name);
            }
            return Flash;
        }
        
        public static string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        public static void AddToAutorun(bool delete = false)
        {
            string FileRoot = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string name = options.GetValue("AutorunName").ToString();
            RegistryKey AutoRun = Registry.CurrentUser
                .OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (delete)
            {
                AutoRun.DeleteValue(name, false);
                AutoRun.Close();
                return;
            }
            foreach (string RegKey in AutoRun.GetValueNames())
            {
                if (RegKey == name)
                {
                    return;
                }
            }
            AutoRun.SetValue(name, FileRoot);
            AutoRun.Close();
        }
        public static void Locker()
        {
            string FileName = options.GetValue("NameKeyFile").ToString();
            string key = options.GetValue("Hash").ToString();
            int timeout = Convert.ToInt32(options.GetValue("TimeoutBlock"));
            while (true)
            {
                List<string> drives = GetFlashka();
                if (drives.Count == 0)
                {
                    System.Diagnostics.Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");
                    Thread.Sleep(timeout);
                    continue;
                }
                bool Verif = false;
                foreach (string s in drives)
                {
                    string fi = s + FileName;
                    if (File.Exists(fi) && File.ReadAllText(fi) == key)
                    {
                        Verif = true;
                        break;
                    }
                }
                if (!Verif)
                {
                    System.Diagnostics.Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");
                }
                Thread.Sleep(timeout);
            }
        }

    }
}
