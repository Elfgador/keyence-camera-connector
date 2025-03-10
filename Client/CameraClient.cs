using System.Data;
using CameraConnector.Command;
using Sres.Net.EEIP;

namespace CameraConnector.Client;

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

        var productName = Client.IdentityObject.ProductName;

        Console.WriteLine($"Connected with : {productName}");
        Console.WriteLine($"Run status : {RunStatusString}");
        IsConnected = true;
    }

    private const int GetStatusAttributeId = 0x28;

    // Assembly Object Constants
    private const int AssemblyObjectClassId = 0x04;
    private const int AssemblyObjectDataAttributeId = 0x03; // ⚠️can't be set with SentOutputInstanceId
    private const int ReceivedOutputInstanceId = 0x65;
    private const int SentOutputInstanceId = 0x64;

    // Assembly Object Data and Response Slot
    // Data slot
    private const int ProgramNumberSlot = 8;

    private const int CommandNumberSlot = 12;

    // Response slot
    private const int RunStatusSlot = 2; // Bit 4

    private EEIPClient Client { get; }
    private static string IpAddress { get; set; } = "192.168.0.1"; // Default ip
    private static ushort UdpPort { get; set; } = 0x08AE; // 2222 Default udp port
    private static ushort TcpPort { get; set; } = 0xAF12; //  44818 Default tcp port
    public bool IsConnected { get; private set; } = false;

    private byte[] Data
    {
        get
        {
            var data = Client.GetAttributeSingle(AssemblyObjectClassId, ReceivedOutputInstanceId,
                AssemblyObjectDataAttributeId);

            if (data == null) throw new DataException();
            return data;
        }
        set => Client.SetAttributeSingle(AssemblyObjectClassId, ReceivedOutputInstanceId,
            AssemblyObjectDataAttributeId, value);
    }

    private byte[] Response
    {
        get
        {
            var response = Client.GetAttributeSingle(AssemblyObjectClassId, SentOutputInstanceId,
                AssemblyObjectDataAttributeId);
            if (response == null) throw new DataException();

            return response;
        }
    }

    public string? ResponseToString() => Response.ToString();

    private int RunStatus // 0: Setup Mode, 1: Run Mode
    {
        get
        {
            var response = Response;
            if (response == null) throw new DataException();
            var statusSlot = response[RunStatusSlot];
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
            Array.Copy(Data, ProgramNumberSlot, programNumberBytes, 0, 4);
            var programNumber = BitConverter.ToUInt32(programNumberBytes, 0);
            return programNumber;
        }
        set
        {
            var programBytes = new byte[4];
            BitConverter.GetBytes(value).CopyTo(programBytes, 0);
            var newData = Data;

            Array.Copy(programBytes, 0, newData, ProgramNumberSlot, 4);

            Data = newData;
        }
    }

    public byte[] CommandNumber
    {
        get
        {
            var commandNumberBytes = new byte[4];

            Array.Copy(Data, CommandNumberSlot, commandNumberBytes, 0, 4);

            return commandNumberBytes;
        }
        set
        {
            var newData = Data;

            Array.Copy(value, 0, newData, CommandNumberSlot, value.Length);

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