using CameraConnector.Client;
using CameraConnector.Command;

class Program
{
    const string CameraIp = "192.168.10.99";
    const string SimulatorIp = "192.168.0.10";

    static async Task Main()
    {
        try
        {
            Console.WriteLine((int)CommandsNumber.Run);
            // Instance the Camera client
            var vsC160Mx = new CameraClient(CameraIp);

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

            while (vsC160Mx.IsConnected)
            {
                Console.WriteLine(vsC160Mx.ToString());

                Console.WriteLine($"Response : {vsC160Mx.ResponseToString()}");

                await Task.Delay(500);
            }

            vsC160Mx.Disconnect();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}