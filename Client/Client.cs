using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;

namespace CameraConnector.Client
{
    public class EtherNetIPClient
    {
        private static IPAddress _IpAddress = IPAddress.Parse("192.168.0.1"); // Default ip
        private static int UPDPort { get; } = 0x08AE; // 2222 Default upd port
        private static int TCPPort { get; } = 0xAF12; // 44818 Default tcp port
        private static Socket? _socket;

        private static byte[] _response = new byte[496];

        public static IPAddress IpAddress
        {
            get { return _IpAddress; }
            set { _IpAddress = value; }
        }
        public static byte[] Response
        {
            get { return _response; }
            private set { _response = value; }
        }

        public static Socket Socket
        {
            get
            {
                return _socket ?? throw new InvalidOperationException("Socket is not initialized.");
            }
            private set { _socket = value; }
        }

        public static void Connect(string ipAddress)
        {
            IpAddress = IPAddress.Parse(ipAddress);
            var endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), TCPPort);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Connect(endpoint);
            Console.WriteLine("Connected to the camera!");
        }

        public static async Task<string> SendCommand(byte[] commandData)
        {
            if (!Socket.Connected)
            {
                throw new InvalidOperationException("Socket is not connected.");
            }

            Socket.Send(commandData);
            Console.WriteLine("Command Sent!");
            return await ReceiveResponse();
        }

        private static async Task<string> ReceiveResponse()
        {
            try
            {
                if (!Socket.Connected)
                {
                    throw new InvalidOperationException("Socket is not connected.");
                }

                byte[] buffer = new byte[1024];
                var bytesReceived = 0;

                while (bytesReceived == 0)
                {
                    Console.WriteLine("Waiting for response...");
                    bytesReceived = Socket.Receive(buffer);
                    await Task.Delay(500);
                }

                Console.WriteLine($"received bytes : {bytesReceived}");

                Response = new byte[bytesReceived];

                Array.Copy(buffer, Response, bytesReceived);

                return ParseResponse(Response);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        private static string ParseResponse(byte[] response)
        {
            return BitConverter.ToString(response);
        }

        public static void CloseConnection()
        {
            Socket.Close();
            Console.WriteLine("Connection closed!");
        }
    }
}
