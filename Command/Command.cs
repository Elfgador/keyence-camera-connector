namespace CameraConnector.Command;

public enum CommandsNumber : byte
{
    Run = 0x69, // 105 This command switches the device from Setup Mode to Run Mode.
    Set = 0x6A, // 106 This command switches the device from Run Mode to Setup Mode.

    Pl = 0x6D, // 109 This command switches the currently
    // set program setting to that of the program setting No. in the specified storage.
    Pr = 0x6E, // 110 This command returns the storage type and program setting number of the current program setting.
    Ec = 0x74, // 116 Echo
}

public enum StorageNumber : byte
{
    Internal = 0x01,
    Sd = 0x02,
}




  