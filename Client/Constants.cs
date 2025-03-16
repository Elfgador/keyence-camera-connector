namespace CameraConnector.Client
{
    public static class VsSeriesConstants
    {
        public const int GetStatusAttributeId = 0x28;

        // Assembly Object Constants
        public const int AssemblyObjectClassId = 0x04;
        public const int AssemblyObjectDataAttributeId = 0x03; // ⚠️can't be set with SentOutputInstanceId
        public const int ReceivedOutputInstanceId = 0x65;
        public const int SentOutputInstanceId = 0x64;

        // Assembly Object Data and Response Slot
        // Data slot
        public const int ProgramNumberSlot = 8;

        public const int CommandNumberSlot = 12;

        // Response slot
        public const int RunStatusSlot = 2; // Bit 4
    }
}