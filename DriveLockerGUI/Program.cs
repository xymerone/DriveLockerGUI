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
        public static IniFile MyCfg;
        public static string FileIniPath = "config.ini";
        public static string sald = "AypiaCgw#N";
        [STAThread]
        
        static void Main()
        {
            
            if (!File.Exists(FileIniPath))
            {
                CreateIniConfig();
            }
            MyCfg = new IniFile(FileIniPath);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }
        static void CreateIniConfig()
        {
            File.WriteAllText(FileIniPath, "");
            var ini = new IniFile(FileIniPath);
            //MyCfg = new IniFile(FileIniPath);
            ini.Write("AutorunName", "DriveLockerGUI");
            ini.Write("Autorun", "false");
            ini.Write("Autostart", "false");
            ini.Write("HideStart", "false");
            ini.Write("TimeoutBlock", "3500");
            ini.Write("Hash",  "null");
            ini.Write("NameKeyFile", "DriveLocker.key");
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
            string name = MyCfg.Read("AutorunName");
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
            string FileName = MyCfg.Read("NameKeyFile");
            string key = MyCfg.Read("Hash");
            int timeout = Convert.ToInt32(MyCfg.Read("TimeoutBlock"));
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
