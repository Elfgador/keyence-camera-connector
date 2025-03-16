using System;
using System.Threading;
using System.Threading.Tasks;
using CameraConnector.Client;
using CameraConnector.Command;
using Sres.Net.EEIP;

namespace CameraConnector
{
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

            var runCommand = vsC160Mx.CreateCommand(CommandsNumber.Run, Array.Empty<byte>());
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
            var vsC160Mx = new EEIPClient();

            vsC160Mx.RegisterSession(ip, tcpPort);

            await Task.Delay(500);

            // Log info
            Console.WriteLine(vsC160Mx.ToString());

            var deviceData = vsC160Mx.GetAttributeSingle(
                VsSeriesConstants.AssemblyObjectClassId,
                VsSeriesConstants.ReceivedOutputInstanceId,
                VsSeriesConstants.AssemblyObjectDataAttributeId);

            await Task.Delay(500);

            BitConverter.GetBytes((byte)1).CopyTo(deviceData, VsSeriesConstants.ProgramNumberSlot);

            Console.WriteLine($"Program Number set to : 1");
            Console.WriteLine("--------------------------------------");

            //  Console.WriteLine($"Camera Status : {(int)deviceData[VsSeriesConstants.RunStatusSlot]}");

            Console.WriteLine("Changing camera status to run...");

            BitConverter.GetBytes((byte)CommandsNumber.Run).CopyTo(deviceData, VsSeriesConstants.RunStatusSlot);

            vsC160Mx.SetAttributeSingle(
                VsSeriesConstants.AssemblyObjectClassId,
                VsSeriesConstants.ReceivedOutputInstanceId,
                VsSeriesConstants.AssemblyObjectDataAttributeId,
                deviceData);

            await Task.Delay(500);

            deviceData = vsC160Mx.GetAttributeSingle(
                VsSeriesConstants.AssemblyObjectClassId,
                VsSeriesConstants.SentOutputInstanceId,
                VsSeriesConstants.AssemblyObjectDataAttributeId);

            var statusSlot = deviceData[VsSeriesConstants.RunStatusSlot];
            var mode = (statusSlot >> 4) & 0x01;

            Console.WriteLine(
                $"Camera Status : {mode}");

            await Task.Delay(500);

            RunKeyListener();

            while (IsRunning)
            {
                deviceData = vsC160Mx.GetAttributeSingle(
                    VsSeriesConstants.AssemblyObjectClassId,
                    VsSeriesConstants.ReceivedOutputInstanceId,
                    VsSeriesConstants.AssemblyObjectDataAttributeId);

                Console.WriteLine(vsC160Mx.ToString());

                Console.WriteLine($"Response : {BitConverter.ToString(deviceData)}");

                await Task.Delay(500);
            }

            vsC160Mx.UnRegisterSession();
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
}