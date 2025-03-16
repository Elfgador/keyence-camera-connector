using CameraConnector.Client;
using CameraConnector.Command;

namespace CameraConnector;

internal static class Program
{
    private const string CameraIp = "192.168.10.99";
    private const string SimulatorIp = "192.168.1.65";
    private const int TcpPort = 44818;

    private static async Task Main()
    {
        try
        {
            await Scripts.ImplicitMessaging(SimulatorIp, TcpPort);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task Personal_try()
    {
        // Instance the Camera client
        var vsC160Mx = new CameraClient(SimulatorIp);

        await Task.Delay(500);

        vsC160Mx.Client.ForwardOpen();

        // Log info
        Console.WriteLine(vsC160Mx.ToString());

        await Task.Delay(500);

        vsC160Mx.ProgramNumber = 1;

        Console.WriteLine($"Program Number set to : {vsC160Mx.ProgramNumber}");
        Console.WriteLine("--------------------------------------");

        Console.WriteLine($"Camera Status : {vsC160Mx.RunStatusString}");

        Console.WriteLine("Changing camera status to run...");
        await Task.Delay(500);

        var runCommand = vsC160Mx.CreateCommand(CommandsNumber.Run, []);
        vsC160Mx.CommandNumber = runCommand;

        Console.WriteLine($"Camera Status : {vsC160Mx.RunStatusString}");

        while (vsC160Mx.IsConnected)
        {
            Console.WriteLine(vsC160Mx.ToString());

            Console.WriteLine($"Response : {vsC160Mx.ResponseToString()}");

            await Task.Delay(500);
        }

        vsC160Mx.Disconnect();
    }
}