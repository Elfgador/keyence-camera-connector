namespace CameraConnector.Command
{
    public enum CommandsNumber : byte
    {
        RUN = 0x69, // 105 This command switches the device from Setup Mode to Run Mode.
        SET = 0x6A, // 106 This command switches the device from Run Mode to Setup Mode.
        PL = 0x6D, // 109 This command switches the currently set program setting to that of the program setting No. in the specified storage.
        PR = 0x6E, // 110 This command returns the storage type and program setting number of the current program setting.
        EC = 0x74, // 116 Echo
    }

    public enum StorageNumber : byte
    {
        Internal = 0x01,
        SD = 0x02,
    }

    public class Command
    {
        public static byte[] ChangeProgramCommand(StorageNumber storageNumber, int programNumber)
        {
            byte[] changeParams = [(byte)storageNumber, (byte)programNumber];

            return CreateCommand(1, CommandsNumber.PL, changeParams);
        }

        public static byte[] CreateCommand(
            int programNumber,
            CommandsNumber commandNumber,
            params byte[] parameters
        )
        {
            byte[] command = new byte[8 + parameters.Length];

            command[0] = 0xF2;
            command[1] = 0x0F;
            command[2] = (byte)programNumber;
            command[3] = 0xF4;
            command[4] = 0x0F;
            command[5] = (byte)commandNumber;

            for (int i = 0; i < parameters.Length; i++)
            {
                command[6 + i] = parameters[i];
            }
            // B078: Activate Command Request
            command[command.Length - 1] = 0x01;

            for (int i = 0; i < command.Length; i++)
            {
                Console.WriteLine($"bytes : {command[i]}");
            }

            return command;
        }

        public static byte[] Create24BytesCommand(
            int programNumber,
            CommandsNumber commandNumber,
            params byte[] parameters
        )
        {
            // Le paquet doit faire 24 octets (8 octets + taille des paramètres)
            byte[] command = new byte[24]; // 24 octets fixes

            command[0] = 0xF2;
            command[1] = 0x0F;
            command[2] = (byte)programNumber;
            command[3] = 0xF4;
            command[4] = 0x0F;
            command[5] = (byte)commandNumber;

            // Ajoute les paramètres à la commande
            for (int i = 0; i < parameters.Length; i++)
            {
                command[6 + i] = parameters[i];
            }

            // Si le paquet est plus petit que 24 octets, compléter avec des octets de remplissage
            if (command.Length < 24)
            {
                command[command.Length - 1] = 0x01; // Le dernier octet d'activation de la commande
            }

            return command;
        }
    }
}
