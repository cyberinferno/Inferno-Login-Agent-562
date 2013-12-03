using System;
using System.Linq;
using System.Text;

namespace Inferno_Login_Agent_562
{
    public static class Packet
    {
        /// <summary>
        /// Returns packet containing custom message in bytes
        /// </summary>
        public static byte[] CreateMessage(string msg)
        {
            var headPacket = new byte[] { 0x5c, 0x00, 0x00, 0x00, 0x01, 0x4f, 0x00, 0x00, 0x01, 0xe0, 0x01 };
            if (msg.Length > 70)
                msg = msg.Substring(0, 69);
            int toFill = 92 - 11 - msg.Length;
            string filler = GetNullString(toFill);
            headPacket = CombineByteArray(headPacket, GetBytesFrom(msg));
            return CombineByteArray(headPacket, GetBytesFrom(filler));
        }

        /// <summary>
        /// Byte array concatination
        /// </summary>
        public static byte[] CombineByteArray(byte[] a, byte[] b)
        {
            var c = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, c, 0, a.Length);
            Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            return c;
        }

        /// <summary>
        /// Returns byte equivalent of a string
        /// </summary>
        private static byte[] GetBytesFrom(string str)
        {
            var encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(str);
            return bytes;
        }

        /// <summary>
        /// Returns a null string of specified length
        /// </summary>
        private static string GetNullString(int length)
        {
            string str = "";
            for (var i = 0; i < length; i++)
                str += char.ConvertFromUtf32(0);
            return str;
        }

        /// <summary>
        /// Create initial packet of a new client while it connects to send to login server
        /// </summary>
        public static byte[] CreateClientIdPacket(int clientId, string ip)
        {
            var packet = new byte[] { 0x1a, 0x00, 0x00, 0x00 };
            var temp = CreateReverseHexPacket(clientId);
            packet = CombineByteArray(packet, temp);
            temp = new byte[] { 0x00, 0x00, 0x02, 0xe1};
            packet = CombineByteArray(packet, temp);
            packet = CombineByteArray(packet, GetBytesFrom(ip));
            return CombineByteArray(packet, ip.Length != 15 ? GetBytesFrom(GetNullString(15 - ip.Length + 1)) : GetBytesFrom(GetNullString(1)));
        }

        /// <summary>
        /// Returns packet for welcome message of the server
        /// </summary>
        public static byte[] CreateWelcomeMessage(int index)
        {
            var packet = new byte[] { 0x5c, 0x00, 0x00, 0x00 };
            packet = CombineByteArray(packet, CreateReverseHexPacket(index));
            var packet1 = new byte[] { 0x00, 0x00, 0x01, 0xe0, 0x00 };
            packet = CombineByteArray(packet, packet1);
            var msg = Config.WelcomeMsg;
            if (msg.Length > 78)
                msg = msg.Substring(0, 78);
            packet = CombineByteArray(packet, GetBytesFrom(msg));
            return CombineByteArray(packet, GetBytesFrom(GetNullString(78 - msg.Length + 3)));
        }

        /// <summary>
        /// Returns packet with server IP and port
        /// </summary>
        public static byte[] CreateLoginAgentIdPacket()
        {
            var packet = new byte[] {0x0b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0xe0};
            string id = string.Format("{0:x}", Config.AgentId);
            var tempByte = new[] {Convert.ToByte(id, 16)};
            return CombineByteArray(packet, tempByte);
        }

        /// <summary>
        /// Returns packet with server port
        /// </summary>
        private static byte[] CreateReverseHexPacket(int port)
        {
            string hexPort = string.Format("{0:x}", port);
            while (hexPort.Length < 4)
                hexPort = "0" + hexPort;
            string temp = hexPort[2] + hexPort[3].ToString();
            string temp1 = hexPort[0] + hexPort[1].ToString();
            var tempByte = new[] { Convert.ToByte(temp, 16), Convert.ToByte(temp1, 16) };
            return tempByte;
        }

        /// <summary>
        /// Returns a packet with client ID which can be sent to login server
        /// </summary>
        public static byte[] CreateLoginServerPacket(byte[] buffer, int read, int cindex)
        {
            byte[] desiredPacket;
            var returnPacket = new byte[] { 0x00 };
            var packetGot = new[] { buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5], buffer[6], buffer[7], buffer[8], buffer[9] };
            switch(read)
            {
                case 56:
                    desiredPacket = new byte[] { 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0xe0 };
                    if (desiredPacket.SequenceEqual(packetGot))
                    {
                        var temp = CreateReverseHexPacket(cindex);
                        buffer[4] = temp[0];
                        buffer[5] = temp[1];
                        returnPacket = buffer;
                    }
                    break;
                case 52:
                    desiredPacket = new byte[] { 0x34, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0xe0 };
                    if (desiredPacket.SequenceEqual(packetGot))
                    {
                        var temp = CreateReverseHexPacket(cindex);
                        buffer[4] = temp[0];
                        buffer[5] = temp[1];
                        returnPacket = buffer;
                    }
                    break;
                case 11:
                    desiredPacket = new byte[] { 0x0b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0xe1 };
                    if (desiredPacket.SequenceEqual(packetGot))
                    {
                        var temp = CreateReverseHexPacket(cindex);
                        buffer[4] = temp[0];
                        buffer[5] = temp[1];
                        returnPacket = buffer;
                    }
                    break;
            }
            return returnPacket;
        }

        /// <summary>
        /// Returns client login acknowledgement packet
        /// </summary>
        public static byte[] CreateAckLsPacket(int index)
        {
            var packet = new byte[] { 0x0a, 0x00, 0x00, 0x00};
            packet = CombineByteArray(packet, CreateReverseHexPacket(index));
            var temp = new byte[] {0x00, 0x00, 0x02, 0xe2};
            return CombineByteArray(packet, temp);
        }

        /// <summary>
        /// Removes extra null bytes from the end of a packet by finding its actual length
        /// </summary>
        public static byte[] TrimPacket(byte[] packet)
        {
            var length = packet[0];
            var newPacket = new byte[] {0x00};
            for (int i = 0; i < length; i++ )
            {
                if (i == 0)
                    newPacket[i] = packet[i];
                else
                {
                    var temp = new[] {packet[i]};
                    newPacket = CombineByteArray(newPacket, temp);
                }
            }
                return newPacket;
        }
    }
}