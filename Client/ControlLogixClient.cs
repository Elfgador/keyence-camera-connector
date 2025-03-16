using System;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace CameraConnector.Client
{
    public class ControlLogixClient : EEIPClient
    {
        public ControlLogixClient(string ipAddress, ushort port = 44818)
        {
            IPAddress = ipAddress;
            TCPPort = port;

            var connectRes = RegisterSession(IPAddress, TCPPort);

            if (connectRes == 0)
            {
                throw new ApplicationException("Failed to register session");
            }

            //Parameters from Originator -> Target | Data
            O_T_InstanceID = 0x66;
            O_T_Length = Detect_O_T_Length();
            O_T_ConnectionType = ConnectionType.Point_to_Point;
            O_T_Priority = Priority.Scheduled;
            O_T_VariableLength = false;
            O_T_OwnerRedundant = false;
            O_T_RealTimeFormat = RealTimeFormat.Modeless;

            //Parameters from Target -> Originator | Response
            T_O_InstanceID = 0x0024;
            T_O_Length = Detect_T_O_Length();
            T_O_ConnectionType = ConnectionType.Point_to_Point;
            T_O_Priority = Priority.Scheduled;
            T_O_VariableLength = false;
            T_O_OwnerRedundant = false;
            T_O_RealTimeFormat = RealTimeFormat.Modeless;

            Console.WriteLine($"Connected with : {IdentityObject.ProductName}");

            IsConnected = true;
        }

        public bool IsConnected { get; private set; } = false;

        public async void Run()
        {
            while (IsConnected)
            {
                var res = T_O_IOData.ToString();
                Console.WriteLine($"Product : {IdentityObject.ProductName}");
                await Task.Delay(500);
                Console.WriteLine(res);
            }
        }
    }
}