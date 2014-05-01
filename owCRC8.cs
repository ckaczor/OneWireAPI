using System;

namespace OneWireAPI
{
	internal class owCRC8
	{
		#region Member variables

		private static byte[]		m_aDataTable;		// Lookup table of CRC8 values

		#endregion

		#region Constructor

		static owCRC8()
		{
			// Initialize the CRC lookup table
			InitializeCRCTable();
		}

		#endregion

		#region Private methods

		private static void InitializeCRCTable()
		{
			int		iAccumulator;		// Accumulator value
			int		iCRC;				// CRC value
			int		iOuter;				// Outer loop control
			int		iInner;				// Inner loop control

			// Initialize the size of the lookup table
			m_aDataTable = new byte[256];

			for (iOuter = 0; iOuter < 256; iOuter++)
			{
				iAccumulator = iOuter;
				iCRC = 0;

				for (iInner = 0; iInner < 8; iInner++)
				{
					if (((iAccumulator ^ iCRC) & 0x01) == 0x01)
						iCRC = ((iCRC ^ 0x18) >> 1) | 0x80;
					else
						iCRC = iCRC >> 1;

					iAccumulator = iAccumulator >> 1;
				}

				m_aDataTable[iOuter] = (byte) iCRC;
			}
		}

		#endregion

		#region Public methods

		public static short Calculate(byte[] nData, int iStart, int iEnd)
		{
			int		iIndex;						// Loop index
			short	nCurrentCRC		= 0;		// Current CRC accumulator

			// Loop over all bytes in the input array
			for (iIndex = iStart; iIndex <= iEnd; iIndex++)
			{
				// Calculate the current CRC for this position
				nCurrentCRC = m_aDataTable[nCurrentCRC ^ nData[iIndex]];
			}

			// Return the final CRC value
			return nCurrentCRC;			
		}

		#endregion
	}
}
