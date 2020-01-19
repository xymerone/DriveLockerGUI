using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DriveLockerGUI
{
    public partial class Form1 : Form
    {
        
        public Thread Lock; // Потік перевірки ключа
        public bool Toggle; // Перемикач для кнопки старт\стоп
        public Form1()
        {
            InitializeComponent();
            RefreshCombo();
            Lock = new Thread(new ThreadStart(Program.Locker));
            Toggle = false;
            
        }

        public void StartConfig()
        {
            if (Program.MyCfg.Read("HideStart") == "true")
            {
                ToTray();
                checkBox2.Checked = true;
            }
            if (Program.MyCfg.Read("Autorun") == "true")
            {
                checkBox1.Checked = true;
            }
            if (Program.MyCfg.Read("Autostart") == "true")
            {
                checkBox3.Checked = true;
                Lock.Start();
                Toggle = true;
                button4.Text = "Стоп";
                ToTray();
            }
        }

        public void RefreshCombo()
        {
            List<string> Rem = Program.GetFlashka();
            comboBox1.Items.Clear();
            if (Rem.Count == 0)
            {
                comboBox1.Items.Add("Немає флешки");
            }
            else
            {
                foreach (string s in Rem)
                {
                    comboBox1.Items.Add(s);
                }
            }
        }

        public void AutoRefreshDevice()
        {
            while( true)
            {
                Thread.Sleep(2000);
                RefreshCombo();
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            if (Toggle)
            {
                Lock.Abort();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                log.Text = "Вкажіть сикрет ключа!!!";
                return;
            }
            string FileKey = (string)comboBox1.SelectedItem + Program.MyCfg.Read("NameKeyFile").ToString();
            if (!File.Exists(FileKey))
            {
                string hs = Program.GetMd5Hash(textBox1.Text + Program.sald);
                textBox1.Text = "";
                File.WriteAllText(FileKey, hs);
                Program.MyCfg.Write("Hash", hs);
                log.Text = "Ключ " + FileKey + " створено!";
                if (!button4.Enabled)
                {
                    button4.Enabled = true;
                    log1.Text = "";
                }
            }
            else
            {
                log.Text = "Ключ вже створено";
            }
        }

        public void ToTray()
        {
            this.ShowInTaskbar = false;
            this.Hide();
            notifyIcon1.Visible = true;
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            this.Show();
                notifyIcon1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ToTray();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Program.AddToAutorun();
                Program.MyCfg.Write("Autorun", "true");
            }
            else
            {
                Program.AddToAutorun(true);
                Program.MyCfg.Write("Autorun", "false");
            }
            if (checkBox2.Checked)
            {
                Program.MyCfg.Write("HideStart", "true");
            }
            else
            {
                Program.MyCfg.Write("HideStart", "false");
            }
            if (checkBox3.Checked)
            {
                Program.MyCfg.Write("Autostart", "true");
            }
            else
            {
                Program.MyCfg.Write("Autostart", "false");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartConfig();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Program.MyCfg.Read("Hash") == "null")
            {
                log1.Text = "Флешка-ключ не створена!!!";
                button4.Enabled = false;
                return;
            }
            if (Toggle)
            {
                Lock.Abort();
                Lock = new Thread(new ThreadStart(Program.Locker));
                button4.Text = "Старт";
                Toggle = false;
            }
            else
            {
                Toggle = true;
                button4.Text = "Стоп";
                Lock.Start();
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Куда клікаєш?? Тут нічого неамє!!!");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RefreshCombo();
        }
    }
}
