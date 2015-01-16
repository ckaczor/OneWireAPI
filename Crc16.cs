namespace OneWireAPI
{
    internal class Crc16
    {
        private static readonly short[] OddParity = { 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0 };

        public static int Calculate(byte[] nData, int iStart, int iEnd)
        {
            return Calculate(nData, iStart, iEnd, 0);
        }

        public static int Calculate(byte nData, int iInitialValue)
        {
            var aData = new byte[1];

            aData[0] = nData;

            return Calculate(aData, 0, 0, iInitialValue);
        }

        public static int Calculate(byte[] nData, int iStart, int iEnd, int iInitialValue)
        {
            int index;								// Loop index
            var currentCrc = iInitialValue;	        // Current CRC accumulator

            // Loop over all bytes in the input array
            for (index = iStart; index <= iEnd; index++)
            {
                // Get the current element of data
                int iBuffer = nData[index];

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
