using System.Net;

namespace Inferno_Login_Agent_562
{
    /// <summary>
    /// Class for saving server configurations
    /// </summary>
    public static class Config
    {
        public static IPAddress LoginServerIp { get; set; }
        public static IPAddress LoginAgentIp { get; set; }
        public static int LoginServerPort { get; set; }
        public static int LoginAgentPort { get; set; }
        public static string DbServerHost { get; set; }
        public static string DbUsername { get; set; }
        public static string DbPassword { get; set; }
        public static string MaintainanceMsg { get; set; }
        public static string WelcomeMsg { get; set; }
        public static int AgentId { get; set; }
        public static bool IsMaintainance { get; set; }
        public static bool IsLoginServerConnected { get; set; }
        public static string LogName { get; set; }
    }
}
