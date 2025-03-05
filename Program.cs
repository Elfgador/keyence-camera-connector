using CameraConnector.Client;
using CameraConnector.Command;

class Program
{
    static readonly string CameraIp = "192.168.10.99";

    static async Task Main()
    {
        try
        {
            // Connection
            EtherNetIPClient.Connect(CameraIp);

            // Run program
            byte[] runCommand = Command.CreateCommand(1, CommandsNumber.RUN, []);
            string res = await EtherNetIPClient.SendCommand(runCommand);

            Console.WriteLine($"Result : {res}");

            // Close connection
            EtherNetIPClient.CloseConnection();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
