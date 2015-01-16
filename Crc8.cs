namespace OneWireAPI
{
    internal class Crc8
    {
        private static byte[] _dataTable;		// Lookup table of CRC8 values

        static Crc8()
        {
            // Initialize the CRC lookup table
            InitializeCrcTable();
        }

        private static void InitializeCrcTable()
        {
            // Initialize the size of the lookup table
            _dataTable = new byte[256];

            for (var outer = 0; outer < 256; outer++)
            {
                var accumulator = outer;    // Accumulator value
                var crc = 0;				// CRC value

                for (var inner = 0; inner < 8; inner++)
                {
                    if (((accumulator ^ crc) & 0x01) == 0x01)
                        crc = ((crc ^ 0x18) >> 1) | 0x80;
                    else
                        crc = crc >> 1;

                    accumulator = accumulator >> 1;
                }

                _dataTable[outer] = (byte) crc;
            }
        }

        public static short Calculate(byte[] data, int start, int end)
        {
            var currentCrc = (short) 0;     // Current CRC accumulator

            // Loop over all bytes in the input array
            for (var index = start; index <= end; index++)
            {
                // Calculate the current CRC for this position
                currentCrc = _dataTable[currentCrc ^ data[index]];
            }

            // Return the final CRC value
            return currentCrc;
        }
    }
}
