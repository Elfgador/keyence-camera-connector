using System;
using System.Data;
using CameraConnector.Command;
using Sres.Net.EEIP;

namespace CameraConnector.Client
{
    public class CameraClient
    {
        public CameraClient(string ipAddress, ushort port = 44818)
        {
            IpAddress = ipAddress;
            TcpPort = port;
            Client = new EEIPClient();
            var connectRes = Client.RegisterSession(IpAddress, TcpPort);

            if (connectRes == 0)
            {
                throw new ApplicationException("Failed to register session");
            }

            Console.WriteLine($"Connected with : {Client.IdentityObject.ProductName}");
            Console.WriteLine($"Status : {RunStatusString}");

            IsConnected = true;
        }


        public EEIPClient Client { get; }
        private static string IpAddress { get; set; } = "192.168.0.1"; // Default ip
        private static ushort UdpPort { get; set; } = 0x08AE; // 2222 Default udp port
        private static ushort TcpPort { get; set; } = 0xAF12; //  44818 Default tcp port
        public bool IsConnected { get; private set; } = false;

        private byte[] Data
        {
            get =>
                Client.GetAttributeSingle(VsSeriesConstants.AssemblyObjectClassId,
                    VsSeriesConstants.ReceivedOutputInstanceId,
                    VsSeriesConstants.AssemblyObjectDataAttributeId) ?? Array.Empty<byte>();

            set => Client.SetAttributeSingle(VsSeriesConstants.AssemblyObjectClassId,
                VsSeriesConstants.ReceivedOutputInstanceId,
                VsSeriesConstants.AssemblyObjectDataAttributeId, value);
        }

        private byte[] Response
        {
            get
            {
                var response = Client.GetAttributeSingle(VsSeriesConstants.AssemblyObjectClassId,
                    VsSeriesConstants.SentOutputInstanceId,
                    VsSeriesConstants.AssemblyObjectDataAttributeId);
                if (response == null) throw new DataException();

                return response;
            }
        }

        public string ResponseToString() => Response.ToString();

        private int RunStatus // 0: Setup Mode, 1: Run Mode
        {
            get
            {
                var response = Response;
                if (response == null) throw new DataException();
                var statusSlot = response[VsSeriesConstants.RunStatusSlot];
                var mode = (statusSlot >> 4) & 0x01;
                return mode;
            }
        }


        public string RunStatusString => RunStatus == 0 ? "Setup Mode" : "Run Mode";

        public uint ProgramNumber
        {
            get
            {
                var programNumberBytes = new byte[4];
                Array.Copy(Data, VsSeriesConstants.ProgramNumberSlot, programNumberBytes, 0, 4);
                var programNumber = BitConverter.ToUInt32(programNumberBytes, 0);
                return programNumber;
            }
            set
            {
                var newData = Data;
                var programBytes = new byte[4];
                BitConverter.GetBytes(value).CopyTo(programBytes, 0);

                Array.Copy(programBytes, 0, newData, VsSeriesConstants.ProgramNumberSlot, 4);
                Data = newData;
            }
        }

        public byte[] CommandNumber
        {
            get
            {
                var commandNumberBytes = new byte[4];

                Array.Copy(Data, VsSeriesConstants.CommandNumberSlot, commandNumberBytes, 0, 4);

                return commandNumberBytes;
            }
            set
            {
                var newData = Data;

                Array.Copy(value, 0, newData, VsSeriesConstants.CommandNumberSlot, value.Length);

                Data = newData;
            }
        }

        public byte[] CreateCommand(CommandsNumber command, byte[] parameters)
        {
            var commandBytes = new byte[4 + parameters.Length];

            BitConverter.GetBytes((int)command).CopyTo(commandBytes, 0);

            Array.Copy(parameters, 0, commandBytes, 4, parameters.Length);

            return commandBytes;
        }

        public override string ToString()
        {
            return $"Product Name : {Client.IdentityObject.ProductName}\n" +
                   $"Mode : {RunStatusString}\n" +
                   $"CommandNumber : {CommandNumber}\n" +
                   $"ProgramNumber : {ProgramNumber}\n";
        }

        public void Disconnect()
        {
            Client.ForwardClose();
            Client.UnRegisterSession();
            IsConnected = false;
        }
    }
}