using System;
using System.Data;
using CameraConnector.Command;
using Sres.Net.EEIP;
using CameraConnector.Client;

namespace CameraConnector.Client
{
    public class VsSeriesClient : EEIPClient
    {
        public VsSeriesClient(string ipAddress, ushort tcpPort = 44818)
        {
            IPAddress = ipAddress;
            TCPPort = tcpPort;

            var connectRes = RegisterSession(IPAddress, TCPPort);

            if (connectRes == 0)
            {
                throw new ApplicationException("Failed to register session");
            }

            //Parameters from Originator -> Target | Data
            O_T_InstanceID = VsSeriesConstants.ReceivedOutputInstanceId;
            O_T_Length = 496;
            O_T_ConnectionType = ConnectionType.Point_to_Point;
            O_T_Priority = Priority.Scheduled;
            O_T_VariableLength = false;
            O_T_OwnerRedundant = false;
            O_T_RealTimeFormat = RealTimeFormat.Modeless;

            //Parameters from Target -> Originator | Response
            // TargetUDPPort = 0x8AE;
            T_O_InstanceID = VsSeriesConstants.SentOutputInstanceId;
            T_O_Length = 496;
            T_O_ConnectionType = ConnectionType.Point_to_Point;
            T_O_Priority = Priority.Scheduled;
            T_O_VariableLength = false;
            T_O_OwnerRedundant = false;
            T_O_RealTimeFormat = RealTimeFormat.Modeless;

            Console.WriteLine($"Connected with : {IdentityObject.ProductName}");
            Console.WriteLine($"Status : {RunStatusString}");

            IsConnected = true;
        }

        public bool IsConnected { get; private set; } = false;

        public string ResponseToString() => T_O_IOData.ToString();


        private bool RunStatus // 0: Setup Mode, 1: Run Mode
        {
            get
            {
                // var mode = (statusSlot >> 4) & 0x01;
                var mode = ToBool(T_O_IOData[VsSeriesConstants.RunStatusSlot], 4);
                return mode;
            }
        }

        public uint ExplicitProgramNumber
        {
            get
            {
                var res = GetAttributeSingle(VsSeriesConstants.AssemblyObjectClassId,
                    VsSeriesConstants.ReceivedOutputInstanceId,
                    VsSeriesConstants.AssemblyObjectDataAttributeId);
                return BitConverter.ToUInt32(res, 0);
            }
            set { }
        }

        public uint ProgramNumber
        {
            get
            {
                var programNumberBytes = new byte[4];
                Array.Copy(O_T_IOData, VsSeriesConstants.ProgramNumberSlot, programNumberBytes, 0, 4);
                var programNumber = BitConverter.ToUInt32(programNumberBytes, 0);
                return programNumber;
            }
            set
            {
                var programBytes = new byte[4];
                BitConverter.GetBytes(value).CopyTo(programBytes, 0);
                var newData = O_T_IOData;

                Array.Copy(programBytes, 0, newData, VsSeriesConstants.ProgramNumberSlot, 4);

                O_T_IOData = newData;
            }
        }

        public byte[] CommandNumber
        {
            get
            {
                var commandNumberBytes = new byte[4];

                Array.Copy(O_T_IOData, VsSeriesConstants.CommandNumberSlot, commandNumberBytes, 0, 4);

                return commandNumberBytes;
            }
            set
            {
                var newData = O_T_IOData;

                Array.Copy(value, 0, newData, VsSeriesConstants.CommandNumberSlot, value.Length);

                O_T_IOData = newData;
            }
        }

        public string RunStatusString => RunStatus is false ? "Setup Mode" : "Run Mode";

        public byte[] CreateCommand(CommandsNumber command, byte[] parameters)
        {
            var commandBytes = new byte[4 + parameters.Length];

            BitConverter.GetBytes((int)command).CopyTo(commandBytes, 0);

            Array.Copy(parameters, 0, commandBytes, 4, parameters.Length);

            return commandBytes;
        }

        public override string ToString()
        {
            return $"Product Name : {IdentityObject.ProductName}\n"
                   + $"Mode : {RunStatusString}\n"
                   + $"CommandNumber : {CommandNumber}\n"
                   + $"ProgramNumber : {ProgramNumber}\n";
        }

        public void Disconnect()
        {
            ForwardClose();
            UnRegisterSession();
            IsConnected = false;
        }
    }
}