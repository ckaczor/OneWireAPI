using System;

namespace OneWireAPI
{
    internal class owCRC16
    {
        #region CRC lookup table

        private static short[] m_aOddParity = { 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0 };

        #endregion

        #region Methods

        public static int Calculate(byte[] nData, int iStart, int iEnd)
        {
            return Calculate(nData, iStart, iEnd, 0);
        }

        public static int Calculate(byte nData, int iInitialValue)
        {
            byte[] aData = new byte[1];

            aData[0] = nData;

            return Calculate(aData, 0, 0, iInitialValue);
        }

        public static int Calculate(byte[] nData, int iStart, int iEnd, int iInitialValue)
        {
            int iIndex;								// Loop index
            int iCurrentCRC = iInitialValue;	// Current CRC accumulator

            // Loop over all bytes in the input array
            for (iIndex = iStart; iIndex <= iEnd; iIndex++)
            {
                // Get the current element of data
                int iBuffer = nData[iIndex];

                // Calculate the current CRC for this position
                iBuffer = (iBuffer ^ (iCurrentCRC & 0xFF)) & 0xFF;

                iCurrentCRC >>= 8;

                if ((m_aOddParity[iBuffer & 0xF] ^ m_aOddParity[iBuffer >> 4]) != 0)
                    iCurrentCRC ^= 0xC001;

                iBuffer <<= 6;
                iCurrentCRC ^= iBuffer;

                iBuffer <<= 1;
                iCurrentCRC ^= iBuffer;
            }

            // Return the final CRC value
            return iCurrentCRC;
        }

        #endregion
    }
}
