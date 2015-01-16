namespace OneWireAPI
{
    internal class Crc16
    {
        private static readonly short[] OddParity = { 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0 };

        public static int Calculate(byte[] data, int start, int end)
        {
            return Calculate(data, start, end, 0);
        }

        public static int Calculate(byte data, int initialValue)
        {
            var bytes = new byte[1];

            bytes[0] = data;

            return Calculate(bytes, 0, 0, initialValue);
        }

        public static int Calculate(byte[] data, int start, int end, int iInitialValue)
        {
            int index;								// Loop index
            var currentCrc = iInitialValue;	        // Current CRC accumulator

            // Loop over all bytes in the input array
            for (index = start; index <= end; index++)
            {
                // Get the current element of data
                int iBuffer = data[index];

                // Calculate the current CRC for this position
                iBuffer = (iBuffer ^ (currentCrc & 0xFF)) & 0xFF;

                currentCrc >>= 8;

                if ((OddParity[iBuffer & 0xF] ^ OddParity[iBuffer >> 4]) != 0)
                    currentCrc ^= 0xC001;

                iBuffer <<= 6;
                currentCrc ^= iBuffer;

                iBuffer <<= 1;
                currentCrc ^= iBuffer;
            }

            // Return the final CRC value
            return currentCrc;
        }
    }
}
