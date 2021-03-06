﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

/*
    Inspired by the program here: https://github.com/10se1ucgo/DisableWinTracking
    I decided to simplify the process and make it in C# because I have a strong disdain for python
    Credit for finding all the registery keys and ips to block goes to the author of the code linked above
    Additional help from this thread: http://answers.microsoft.com/en-us/insider/forum/insider_wintp-insider_files/how-to-uninstall-onedrive-completely-in-windows-10/e735a3b8-09f1-40e2-89c3-b93cf7fe6994?auth=1
*/


namespace Windows_10_Privacy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.disableAutoLogger();
            this.disableTelemetry();
            this.blockHosts();
            this.stopDmwappushsvc();
            this.stopTrackingService();
            if(this.checkBox1.CheckState == CheckState.Checked)
                this.disableOneDrive();
            MessageBox.Show("Done");
        }

        private void disableOneDrive()
        {
            this.logInfo("Disabling OneDrive");
            Process[] procs = Process.GetProcessesByName("OneDrive");
            if (procs.Length > 0)
            {
                Process process = procs[0];
                if (process != null)
                {
                    process.Kill();
                }
            }

            string x64Path = "C:\\Windows\\SysWOW64";


            try
            {
                Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run", "OneDrive", "");
                this.logInfo("Deleting OneDrive");
                if (!File.Exists(x64Path))
                {
                    RegistryKey reg = Registry.ClassesRoot.OpenSubKey("CLSID", true);
                    if (reg != null)
                        reg.DeleteSubKeyTree("{018D5C66-4533-4307-9B53-224DE2ED1FE6}");
                }
                else if(File.Exists(x64Path))
                {
                    RegistryKey reg2 = Registry.ClassesRoot.OpenSubKey("Wow6432Node\\CLSID", true);
                    if (reg2 != null)
                        reg2.DeleteSubKeyTree("{018D5C66-4533-4307-9B53-224DE2ED1FE6}");
                }
                
               
            }
            catch (Exception ex)
            {
                this.logInfo("Could not disable OneDrive");
            }
            try
            {
                string user = Environment.UserName;
                string path = @"C:\Users\" + user + @"\AppData\Local\Microsoft\OneDrive\";
                this.logInfo(path);
                 foreach(string file in Directory.GetFiles(path))
                {
                    this.logInfo(file);
                    FileStream theFile = File.Open(file, FileMode.Open);
                    if(theFile.CanWrite)
                    {
                        theFile.Close();
                        theFile = null;
                        File.Delete(file);
                    }
                    if(theFile != null)
                     theFile.Close();
                }
            }
            catch(IOException e)
            {
                Console.Write(e.StackTrace);
            }
        }

        private void disableAutoLogger()
        {
            this.logInfo("Disabling AutoLogger");
            try
            {
                FileStream stream = File.Open("C:\\ProgramData\\Microsoft\\Diagnosis\\ETLLogs\\AutoLogger\\AutoLogger-Diagtrack-Listener.etl", FileMode.Open);
                stream.Close();
                System.Diagnostics.Process.Start("echo y | cacls C:\\ProgramData\\Microsoft\\Diagnosis\\ETLLogs\\AutoLogger\\AutoLogger - Diagtrack - Listener.etl / d SYSTEM");
            }
            catch(IOException ex)
            {
                this.logInfo("Autologger not found skipping");
            }
        }

        private void disableTelemetry()
        {
            this.logInfo("Disabling telemtry");
            try {
                Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry", 0);
            }
            catch(Exception ex)
            {
                this.logInfo("Could not disable telemetry");
            }
        }

        private void blockHosts()
        {
            this.logInfo("Blocking hosts in host file");
            string line = Environment.NewLine;
            string hosts = "0.0.0.0 vortex.data.microsoft.com" + line + "0.0.0.0 vortex-win.data.microsoft.com"  + line +  
                "0.0.0.0 telecommand.telemetry.microsoft.com"  + line +  "0.0.0.0 telecommand.telemetry.microsoft.com.nsatc.net"  + line +  
                "0.0.0.0 oca.telemetry.microsoft.com"  + line +  "0.0.0.0 oca.telemetry.microsoft.com.nsatc.net"  + line +  
                "0.0.0.0 sqm.telemetry.microsoft.com"  + line +  "0.0.0.0 sqm.telemetry.microsoft.com.nsatc.net"  + line +  
                "0.0.0.0 watson.telemetry.microsoft.com"  + line +  "0.0.0.0 watson.telemetry.microsoft.com.nsatc.net"  + line +  
                "0.0.0.0 redir.metaservices.microsoft.com"  + line +  "0.0.0.0 choice.microsoft.com"  + line +  
                "0.0.0.0 choice.microsoft.com.nsatc.net"  + line +  "0.0.0.0 df.telemetry.microsoft.com"  + line +  
                "0.0.0.0 reports.wes.df.telemetry.microsoft.com"  + line +  "0.0.0.0 wes.df.telemetry.microsoft.com"  + line +  
                "0.0.0.0 services.wes.df.telemetry.microsoft.com"  + line +  "0.0.0.0 sqm.df.telemetry.microsoft.com"  + line +  
                "0.0.0.0 telemetry.microsoft.com"  + line +  "0.0.0.0 watson.ppe.telemetry.microsoft.com"  + line +  
                "0.0.0.0 telemetry.appex.bing.net"  + line +  "0.0.0.0 telemetry.urs.microsoft.com"  + line +  
                "0.0.0.0 telemetry.appex.bing.net:443"  + line +  "0.0.0.0 settings-sandbox.data.microsoft.com"  + line +  
                "0.0.0.0 vortex-sandbox.data.microsoft.com"  + line +  "0.0.0.0 survey.watson.microsoft.com"  + line +  
                "0.0.0.0 watson.live.com"  + line +  "0.0.0.0 watson.microsoft.com"  + line +  "0.0.0.0 statsfe2.ws.microsoft.com"  + line +  
                "0.0.0.0 corpext.msitadfs.glbdns2.microsoft.com"  + line +  "0.0.0.0 compatexchange.cloudapp.net"  + line +  
                "0.0.0.0 cs1.wpc.v0cdn.net"  + line +  "0.0.0.0 a-0001.a-msedge.net"  + line +  
                "0.0.0.0 statsfe2.update.microsoft.com.akadns.net"  + line +  "0.0.0.0 sls.update.microsoft.com.akadns.net"  + line +  
                "0.0.0.0 fe2.update.microsoft.com.akadns.net"  + line +  "0.0.0.0 65.55.108.23 "  + line +  "0.0.0.0 65.39.117.230"  + line +  
                "0.0.0.0 23.218.212.69 "  + line +  "0.0.0.0 134.170.30.202"  + line +  "0.0.0.0 137.116.81.24"  + line +  
                "0.0.0.0 diagnostics.support.microsoft.com"  + line +  "0.0.0.0 corp.sts.microsoft.com"  + line +  
                "0.0.0.0 statsfe1.ws.microsoft.com"  + line +  "0.0.0.0 pre.footprintpredict.com"  + line +  "0.0.0.0 204.79.197.200"  + line +  
                "0.0.0.0 23.218.212.69"  + line +  "0.0.0.0 i1.services.social.microsoft.com"  + line +  
                "0.0.0.0 i1.services.social.microsoft.com.nsatc.net"  + line +  "0.0.0.0 feedback.windows.com"  + line +  
                "0.0.0.0 feedback.microsoft-hohm.com"  + line +  "0.0.0.0 feedback.search.microsoft.com";
            try {
                StreamWriter writer = File.AppendText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts"));
                writer.Write(hosts);
                writer.Close();
            }
            catch(IOException e)
            {
                this.logInfo("Could not block hosts");
            }
        }

        private void stopDmwappushsvc()
        {
            this.logInfo("Stopping dmwappushsvc if running");
            Process[] procs = Process.GetProcessesByName("dmwappushsvc");
            if (procs.Length > 0)
            {
                Process process = procs[0];
                if (process != null)
                {
                    process.Kill();
                }
            }
            this.logInfo("Disabling dmwappushsvc");
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\dmwappushsvc", "Start", 0);
        }

        private void stopTrackingService()
        {
            this.logInfo("Stopping Diagnostics Tracking Service if running");
            Process[] procs = Process.GetProcessesByName("Diagnostics Tracking Service");
            if (procs.Length > 0)
            {
                Process process = procs[0];
                if (process != null)
                {
                    process.Kill();
                }
            }
            this.logInfo("Disabling tracking service");
            Registry.SetValue("HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\DiagTrack", "Start", 0);
        }


        private void logInfo(string info)
        {
            this.richTextBox1.AppendText("[" + System.DateTime.UtcNow.ToString() + "] " + info + "\n");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://onedrive.live.com/about/en-us/download/");
        }
    }
}
