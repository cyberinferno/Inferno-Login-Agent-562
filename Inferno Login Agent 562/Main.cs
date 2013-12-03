using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Inferno_Login_Agent_562
{
    public partial class Main : Form
    {
        private LoginAgent _loginAgent;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Load default config variables
            Config.LoginServerPort = 3550;
            Config.LoginAgentPort = 3210;
            Config.LoginServerIp = IPAddress.Parse("127.0.0.1");
            Config.LoginAgentIp = IPAddress.Any;
            Config.DbServerHost = "(local)";
            Config.DbUsername = "sa";
            Config.DbPassword = "ley";
            Config.MaintainanceMsg = "Server is down for maintainance!";
            Config.WelcomeMsg = "Welcome to A3";
            Config.AgentId = 0;
            Config.IsMaintainance = false;
            Config.IsLoginServerConnected = false;
            Config.LogName = GenerateFileName("LoginAgent") + ".log";
            if (!File.Exists("Svrinfo.ini"))
            {
                MessageBox.Show("Svrinfo.ini not found!", "Inferno Login Agent 562 error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            else
            {
                // Read ini file and load details
                using (var streamReader = new StreamReader("Svrinfo.ini", true))
                {
                    string readLine;
                    while ((readLine = streamReader.ReadLine()) != null)
                    {
                        var config = readLine.Split(':');
                        if(config.Length != 2)
                            continue;
                        switch (config[0])
                        {
                            case "LoginServerIp":
                                Config.LoginServerIp = IPAddress.Parse(config[1].Trim());
                                break;
                            case "LoginServerPort":
                                Config.LoginServerPort = Convert.ToInt32(config[1].Trim());
                                break;
                            case "LoginAgentIp":
                                Config.LoginAgentIp = IPAddress.Parse(config[1].Trim());
                                break;
                            case "LoginAgentPort":
                                Config.LoginAgentPort = Convert.ToInt32(config[1].Trim());
                                break;
                            case "DatabaseHost":
                                Config.DbServerHost = config[1].Trim();
                                break;
                            case "DatabaseUsername":
                                Config.DbUsername = config[1].Trim();
                                break;
                            case "DatabasePassword":
                                Config.DbPassword = config[1].Trim();
                                break;
                            case "MaintainanceMsg":
                                Config.MaintainanceMsg = config[1].Trim();
                                break;
                            case "AgentId":
                                Config.AgentId = Convert.ToInt32(config[1].Trim());
                                break;
                            case "WelcomeMsg":
                                Config.WelcomeMsg = config[1].Trim();
                                break;
                        }
                    }
                }
                try
                {
                    // Start timer
                    lsTimer.Start();
                    // Initialize login agent
                    _loginAgent = new LoginAgent();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("ERROR : " + ex.Message, "Inferno Login Agent 562 error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    Logger.WriteLog("Main.cs error : " + ex);
                }
            }
        }

        private void maintainanceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (maintainanceCheckBox.Checked)
                Config.IsMaintainance = true;
            else
                Config.IsMaintainance = false;
        }

        private void lsTimer_Tick(object sender, EventArgs e)
        {
            lsStatus.Text = Config.IsLoginServerConnected ? "Connected" : "Disconnected";
        }

        private static string GenerateFileName(string context)
        {
            return context + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
    }
}
