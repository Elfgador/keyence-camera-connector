using CameraConnector.Client;
using CameraConnector.Command;

namespace CameraConnector;

public static class Scripts
{
    private static bool IsRunning { get; set; } = false;

    public static async Task ImplicitMessaging(string ip, ushort tcpPort)
    {
        // Instance the Camera client
        var vsC160Mx = new VsSeriesClient(ip, tcpPort);

        await Task.Delay(500);

        vsC160Mx.ForwardOpen();
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

        RunKeyListener();

        while (vsC160Mx.IsConnected && IsRunning)
        {
            Console.WriteLine(vsC160Mx.ToString());

            Console.WriteLine($"Response : {vsC160Mx.ResponseToString()}");

            await Task.Delay(500);
        }

        vsC160Mx.ForwardClose();
        vsC160Mx.Disconnect();
    }

    public static async Task ExplicitMessaging(string ip, ushort tcpPort)
    {
        // Instance the Camera client
        var vsC160Mx = new VsSeriesClient(ip, tcpPort);

        await Task.Delay(500);

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

        RunKeyListener();

        while (vsC160Mx.IsConnected && IsRunning)
        {
            Console.WriteLine(vsC160Mx.ToString());

            Console.WriteLine($"Response : {vsC160Mx.ResponseToString()}");

            await Task.Delay(500);
        }

        vsC160Mx.ForwardClose();
        vsC160Mx.Disconnect();
    }

    private static void RunKeyListener()
    {
        IsRunning = true;

        var keyListener = new Thread(() =>
        {
            Console.WriteLine("Press on 'q' to leave");
            while (true)
            {
                if (Console.ReadKey(true).Key != ConsoleKey.Q) continue;
                IsRunning = false;
                break;
            }
        });
        keyListener.Start();
    }
}