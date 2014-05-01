using System;

namespace OneWireAPI
{
	public class owDeviceFamily1D : owDevice
	{
		#region Constructor

		public owDeviceFamily1D(owSession Session, short[] ID) : base(Session, ID)
		{
			// Just call the base constructor
		}

		#endregion

		#region Methods

		public uint GetCounter(int CounterPage)
		{
			short		nResult;								// Result of method calls
			byte[]		aData				= new byte[30];		// Data buffer to send over the network
			short		nDataCount			= 0;				// How many bytes of data to send
			int			iLastByte;								// Address of the last byte in the requested page
			uint		iCounter			= 0;				// Counter value
			int			iCRCResult;								// Result of the CRC calculation
			int			iMatchCRC;								// CRC retrieved from the device

			// Select and access the ID of the device we want to talk to
			owAdapter.Select(_deviceID);

			// Set the "read memory and counter" command into the data array
			aData[nDataCount++] = 0xA5;

			// Calculate the position of the last byte in the page
			iLastByte = (CounterPage << 5) + 31;

			// Copy the lower byte of the last byte into the data array
			aData[nDataCount++] = (byte) (iLastByte & 0xFF);

			// Copy the upper byte of the last byte into the data array
			aData[nDataCount++] = (byte) (iLastByte >> 8);

			// Add byte for the data byate, counter, zero bits, and CRC16 result
			for (int i = 0; i < 11; i++)	aData[nDataCount++] = 0xFF;

			// Send the block of data to the device
			nResult = owAdapter.SendBlock(aData, nDataCount);

			// Calculate the CRC based on the data
			iCRCResult = owCRC16.Calculate(aData, 0, 11);

			// Assemble the CRC provided by the device
			iMatchCRC = aData[13] << 8;
			iMatchCRC |= aData[12];
			iMatchCRC ^= 0xFFFF;

			// Make sure the CRC values match
			if (iCRCResult != iMatchCRC)
			{
				// Throw a CRC exception
				throw new owException(owException.owExceptionFunction.CRC, _deviceID);
			}

			// Assemble the counter data from the bytes retrieved
			for (int i = nDataCount - 7; i >= nDataCount - 10; i--)
			{
				iCounter <<= 8;
				iCounter |= aData[i];
			}

			return iCounter;
		}

		#endregion
	}
}
