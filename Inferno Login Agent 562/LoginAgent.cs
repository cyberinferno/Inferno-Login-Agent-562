using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace Inferno_Login_Agent_562
{
    public class LoginAgent : IDisposable
    {
        private readonly List<Client> _clients;
        private TcpListener _listener;
        public bool Running;
        private List<int> _referenceIdHolder;
        private Dictionary<string, int> _clientInfoHolder;
        private readonly TcpClient _loginServerConnection;
        private NetworkStream _loginServerNetworkStream;
        private Timer _lsConnectionChecker;

        /// <summary>
        /// Constructor to intialize variables for the server
        /// </summary>
        public LoginAgent()
        {
            _loginServerConnection = new TcpClient { NoDelay = true };
            _loginServerConnection.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _listener = new TcpListener(Config.LoginAgentIp, Config.LoginAgentPort);
            _lsConnectionChecker = new Timer(500);
            _lsConnectionChecker.Elapsed += CheckLoginServerConnection;
            _lsConnectionChecker.Start();
            Encoding = Encoding.Default;
            _clients = new List<Client>();
            _referenceIdHolder = new List<int>();
            _clientInfoHolder = new Dictionary<string, int>();
            Running = false;
        }

        /// <summary>
        /// Check for login server connection and start/stop login agent listening
        /// </summary>
        private void CheckLoginServerConnection(object sender, ElapsedEventArgs e)
        {
            if(_loginServerConnection.Connected)
            {
                if(!Running)
                    Start();
                Config.IsLoginServerConnected = true;
            }
            else
            {
                if(Running)
                    Stop();
                Config.IsLoginServerConnected = false;
                try
                {
                    _loginServerConnection.Connect(Config.LoginServerIp, Config.LoginServerPort);
                    if (_loginServerConnection.Connected)
                    {
                        Config.IsLoginServerConnected = true;
                        _loginServerNetworkStream = _loginServerConnection.GetStream();
                        _loginServerNetworkStream.Write(Packet.CreateLoginAgentIdPacket(), 0, Packet.CreateLoginAgentIdPacket().Length);
                        _loginServerNetworkStream.Flush();
                        Start();
                    }
                }
                catch(Exception ex)
                {
                    Logger.WriteLog("Error in CheckLoginServerConnection! Msg : " + ex);
                }
            }
        }

        /// <summary>
        /// The encoding to use when sending / receiving strings.
        /// </summary>
        private Encoding Encoding { get; set; }

        /// <summary>
        /// An enumerable collection of all the currently connected tcp clients
        /// </summary>
        public IEnumerable<TcpClient> TcpClients
        {
            get
            {
                return _clients.Select(client => client.TcpClient);
            }
        }

        /// <summary>
        /// Starts the TCP Server listening for new clients.
        /// </summary>
        public void Start()
        {
            if (Config.IsLoginServerConnected)
            {
                _listener.Start();
                _listener.BeginAcceptTcpClient(ClientHandler, null);
                Running = true;
            }
        }

        /// <summary>
        /// Stops the TCP Server listening for new clients and disconnects
        /// any currently connected clients.
        /// </summary>
        public void Stop()
        {
            _listener.Stop();
            lock (_clients)
            {
                foreach (Client client in _clients)
                {
                    client.TcpClient.Client.Disconnect(false);
                }
                _clients.Clear();
            }
            Running = false;
        }

        /// <summary>
        /// Writes a string to a given TCP Client
        /// </summary>
        /// <param name="tcpClient">The client to write to</param>
        /// <param name="data">The string to send.</param>
        private void Write(TcpClient tcpClient, string data)
        {
            byte[] bytes = Encoding.GetBytes(data);
            Write(tcpClient, bytes);
        }

        /// <summary>
        /// Writes a string to all clients connected.
        /// </summary>
        /// <param name="data">The string to send.</param>
        public void Write(string data)
        {
            foreach (Client client in _clients)
            {
                Write(client.TcpClient, data);
            }
        }

        /// <summary>
        /// Writes a byte array to all clients connected.
        /// </summary>
        /// <param name="bytes">The bytes to send.</param>
        public void Write(byte[] bytes)
        {
            foreach (Client client in _clients)
            {
                Write(client.TcpClient, bytes);
            }
        }

        /// <summary>
        /// Writes a byte array to a given TCP Client
        /// </summary>
        /// <param name="tcpClient">The client to write to</param>
        /// <param name="bytes">The bytes to send</param>
        private static void Write(TcpClient tcpClient, byte[] bytes)
        {
            NetworkStream networkStream = tcpClient.GetStream();
            networkStream.BeginWrite(bytes, 0, bytes.Length, WriteCallback, tcpClient);
        }

        /// <summary>
        /// Callback for the write opertaion.
        /// </summary>
        private static void WriteCallback(IAsyncResult result)
        {
            try
            {
                var tcpClient = result.AsyncState as TcpClient;
                if (tcpClient != null)
                {
                    NetworkStream networkStream = tcpClient.GetStream();
                    networkStream.EndWrite(result);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Write call back exception : " + ex.Message);
            }
        }

        /// <summary>
        /// Callback for the accept tcp client operation.
        /// </summary>
        private void ClientHandler(IAsyncResult asyncResult)
        {
            try
            {
                TcpClient client = _listener.EndAcceptTcpClient(asyncResult);
                var buffer = new byte[client.ReceiveBufferSize];
                var newClient = new Client(client, buffer);
                lock (_clients)
                {
                    _clients.Add(newClient);
                }
                NetworkStream networkStream = newClient.NetworkStream;
                networkStream.BeginRead(newClient.Buffer, 0, newClient.Buffer.Length, OnDataRead, newClient);
                _listener.BeginAcceptTcpClient(ClientHandler, null);
            }
            catch (Exception e)
            {
                Logger.WriteLog("ClientHandler ERROR: " + e.Message);
            }
        }

        /// <summary>
        /// Callback for the read opertaion.
        /// </summary>
        private void OnDataRead(IAsyncResult asyncResult)
        {
            var client = asyncResult.AsyncState as Client;
            try
            {
                if (client == null)
                    return;
                NetworkStream networkStream = client.NetworkStream;
                var newClientEp = (IPEndPoint)client.TcpClient.Client.RemoteEndPoint;
                // Check if IP is banned
                if (!IsIpBanned(newClientEp.Address.ToString()))
                {
                    // Check for server maintainance
                    if (!Config.IsMaintainance)
                    {
                        int read = networkStream.EndRead(asyncResult);
                        if (read == 0)
                        {
                            lock (_clients)
                            {
                                _clients.Remove(client);
                                return;
                            }
                        }
                        int cindex;
                        byte[] toSend;
                        // Check for existence of a client
                        if (!_clientInfoHolder.ContainsKey(newClientEp.Address + ":" + newClientEp.Port))
                        {
                            // Create and save unique ID for a client
                            cindex = GetUniqueClientId();
                            _referenceIdHolder.Add(cindex);
                            _clientInfoHolder.Add(newClientEp.Address + ":" + newClientEp.Port, cindex);
                            toSend = Packet.CreateClientIdPacket(cindex, newClientEp.Address.ToString());
                            // Register a new client with login server
                            _loginServerNetworkStream.Write(toSend, 0, toSend.Length);
                            _loginServerNetworkStream.Flush();
                        }
                        else
                            cindex = _clientInfoHolder[newClientEp.Address + ":" + newClientEp.Port];
                        toSend = Packet.CreateLoginServerPacket(client.Buffer, read, cindex);
                        // Send packet from client to login server after adding client ID
                        _loginServerNetworkStream.Write(toSend, 0, read);
                        _loginServerNetworkStream.Flush();
                        var bytes = new byte[1024];
                        // Read data arrived from login server
                        _loginServerNetworkStream.Read(bytes, 0, 1024);
                        // Parse data from login server
                        if (Encoding.ASCII.GetString(bytes).Substring(14, 7) == "Invalid")
                        {
                            // Destroy client ID as the credentials were invalid
                            var index = _clientInfoHolder[newClientEp.Address + ":" + newClientEp.Port];
                            _clientInfoHolder.Remove(newClientEp.Address + ":" + newClientEp.Port);
                            _referenceIdHolder.Remove(index);
                        }
                        else if (Encoding.ASCII.GetString(bytes).Substring(30, 6) == "ONLINE")
                        {
                            // Prepend welcome message to the server name data
                            bytes = Packet.CombineByteArray(Packet.CreateWelcomeMessage(cindex), Packet.TrimPacket(bytes));
                        }
                        else if (Encoding.ASCII.GetString(bytes).Substring(12, 4) == "User")
                        {
                            // Destroy client ID as the user has already logged in
                            var index = _clientInfoHolder[newClientEp.Address + ":" + newClientEp.Port];
                            _clientInfoHolder.Remove(newClientEp.Address + ":" + newClientEp.Port);
                            _referenceIdHolder.Remove(index);
                        }
                        else if (Encoding.ASCII.GetString(bytes).Substring(11, 7) == "Account")
                        {
                            // Destroy client ID as the account is already active
                            var index = _clientInfoHolder[newClientEp.Address + ":" + newClientEp.Port];
                            _clientInfoHolder.Remove(newClientEp.Address + ":" + newClientEp.Port);
                            _referenceIdHolder.Remove(index);
                        }
                        else
                        {
                            _loginServerNetworkStream.Flush();
                            toSend = Packet.CreateAckLsPacket(cindex);
                            // Acknowledge login server to create a login for the client ID
                            _loginServerNetworkStream.Write(toSend, 0, toSend.Length);
                        }
                        _loginServerNetworkStream.Flush();
                        // Trim packet if needed
                        if(Encoding.ASCII.GetString(bytes).Substring(122, 6) != "ONLINE")
                            Write(client.TcpClient, Packet.TrimPacket(bytes));
                        else
                            Write(client.TcpClient, bytes);
                    }
                    else
                        Write(client.TcpClient, Packet.CreateMessage(Config.MaintainanceMsg));
                }
                else
                    Write(client.TcpClient, Packet.CreateMessage("Your IP is banned.Contact admin"));
                networkStream.BeginRead(client.Buffer, 0, client.Buffer.Length, OnDataRead, client);
            }
            catch (Exception e)
            {
                Logger.WriteLog("OnDataRead error : " + e.Message);
            }
        }

        /// <summary>
        /// Checks whether an IP is banned
        /// </summary>
        private static bool IsIpBanned(string ip)
        {
            if (File.Exists("ipbanlist.txt"))
            {
                try
                {
                    var ipList = new List<string>();
                    using (var streamReader = new StreamReader("ipbanlist.txt", true))
                    {
                        string readLine;
                        while ((readLine = streamReader.ReadLine()) != null)
                            ipList.Add(readLine.Trim());
                    }
                    return ipList.ToArray().Any(temp => String.Equals(temp, ip));
                }
                catch (Exception ex)
                {
                    Logger.WriteLog("IP ban check error : " + ex.Message);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns an unique ID for a connected client
        /// </summary>
        private int GetUniqueClientId()
        {
            int rand;
            while (true)
            {
                rand = (new Random()).Next(1, 65000);
                if (!_referenceIdHolder.ToArray().Any(temp => temp == rand))
                    break;
            }
            return rand;
        }

        /// <summary>
        /// Release resources used by login server connection
        /// </summary>
        public void Dispose()
        {
            _loginServerConnection.Client.Dispose();
            _loginServerNetworkStream.Dispose();
        }
    }
}
