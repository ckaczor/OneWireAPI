using System;

namespace OneWireAPI
{
	public class owDeviceFamily20 : owDevice
	{
		#region Member variables

		byte[]		m_aControl	=	new byte[16];

		#endregion

		#region Enumerations

		public enum owDeviceFamily20Range : byte
		{
			Range_512		=	0x01,
			Range_256		=	0x00
		}	

		public enum owDeviceFamily20Resolution : byte
		{
			EightBits		=	0x08,
			SixteenBits		=	0x00
		}

		#endregion

		#region Constructor

		public owDeviceFamily20(owSession Session, short[] ID) : base(Session, ID)
		{
		}

		#endregion

		#region Methods

		public void Initialize()
		{
			short		nResult;								// Result of method calls
			byte[]		aData				= new byte[30];		// Data buffer to send over the network
			short		nDataCount			= 0;				// How many bytes of data to send
			int			iIndex;									// Loop index
			int			iStartAddress		= 0x8;				// Starting data page address
			int			iEndAddress			= 0x11;				// Ending data page address
			int			iCalculatedCRC;							// CRC we calculated from sent data
			int			iSentCRC;								// CRC retrieved from the device

			// Setup the control page
			for (iIndex = 0; iIndex < 8; iIndex += 2)
			{
				m_aControl[iIndex] = (byte) owDeviceFamily20Resolution.EightBits;
				m_aControl[iIndex + 1] = (byte) owDeviceFamily20Range.Range_512;
			}

			// Clear the alarm page
			for (iIndex = 8; iIndex < 16; iIndex++)
			{
				m_aControl[iIndex] = 0;
			}

			// Set the command into the data array
			aData[nDataCount++] = 0x55;

			// Set the starting address of the data to write
			aData[nDataCount++] = (byte) (iStartAddress & 0xFF);
			aData[nDataCount++] = (byte) ((iStartAddress >> 8) & 0xFF);

			// Select and access the ID of the device we want to talk to
			owAdapter.Select(_deviceID);

			// Write to the data pages specified
			for (iIndex = iStartAddress; iIndex <= iEndAddress; iIndex++)
			{
				// Copy the control data into our output buffer
				aData[nDataCount++] = m_aControl[iIndex - iStartAddress];

				// Add two bytes for the CRC results
				aData[nDataCount++] = 0xFF;
				aData[nDataCount++] = 0xFF;

				// Add a byte for the control byte echo
				aData[nDataCount++] = 0xFF;
				
				// Send the block
				nResult = owAdapter.SendBlock(aData, nDataCount);

				// If the check byte doesn't match then throw an exception
				if (aData[nDataCount - 1] != m_aControl[iIndex - iStartAddress])
				{
					// Throw an exception
					throw new owException(owException.owExceptionFunction.SendBlock, _deviceID);
				}

				// Calculate the CRC values
				if (iIndex == iStartAddress)
				{
					// Calculate the CRC16 of the data sent
					iCalculatedCRC = owCRC16.Calculate(aData, 0, 3);

					// Reconstruct the CRC sent by the device
					iSentCRC = aData[nDataCount - 2] << 8;
					iSentCRC |= aData[nDataCount - 3];
					iSentCRC ^= 0xFFFF;
				}
				else
				{
					// Calculate the CRC16 of the data sent
					iCalculatedCRC = owCRC16.Calculate(m_aControl[iIndex - iStartAddress], iIndex);

					// Reconstruct the CRC sent by the device
					iSentCRC = aData[nDataCount - 2] << 8;
					iSentCRC |= aData[nDataCount - 3];
					iSentCRC ^= 0xFFFF;
				}

				// If the CRC doesn't match then throw an exception
				if (iCalculatedCRC != iSentCRC)
				{
					// Throw a CRC exception
					throw new owException(owException.owExceptionFunction.CRC, _deviceID);
				}

				// Reset the byte count
				nDataCount = 0;
			}
		}

		public double[] GetVoltages()
		{
			short		nResult;								// Result of method calls
			byte[]		aData				= new byte[30];		// Data buffer to send over the network
			short		nDataCount			= 0;				// How many bytes of data to send
			int			iCalculatedCRC;							// CRC we calculated from sent data
			int			iSentCRC;								// CRC retrieved from the device
			short		nTransmitByte;							// Byte of data with the strong pull-up
			short		nCheckByte			= 0;				// Byte of data read to see if conversion is done
			int			iIndex;									// Loop index
			int			iVoltageReadout;						// Restructed voltage value from memory
			double[]	dVoltages			= new double[4];	// Voltage values to return

			// Select and access the ID of the device we want to talk to
			owAdapter.Select(_deviceID);

			// Set the convert command into the transmit buffer
			aData[nDataCount++] = 0x3C;

			// Set the input mask to get all channels
			aData[nDataCount++] = 0x0F;

			// Set the read-out control to leave things as they are
			aData[nDataCount++] = 0x00;

			// Add two bytes for the CRC results
			aData[nDataCount++] = 0xFF;
			aData[nDataCount++] = 0xFF;

			// Send the data block
			owAdapter.SendBlock(aData, nDataCount);

			// Calculate the CRC based on the transmit buffer
			iCalculatedCRC = owCRC16.Calculate(aData, 0, 2);

			// Reconstruct the CRC sent by the device
			iSentCRC = aData[4] << 8;
			iSentCRC |= aData[3];
			iSentCRC ^= 0xFFFF;

			// If the CRC doesn't match then throw an exception
			if (iCalculatedCRC != iSentCRC)
			{
				// Throw a CRC exception
				throw new owException(owException.owExceptionFunction.CRC, _deviceID);
			}

			// Setup for for power delivery after the next byte
			nResult = owAdapter.SetLevel(TMEX.TMOneWireLevelOperation.Write, TMEX.TMOneWireLevelMode.StrongPullup, TMEX.TMOneWireLevelPrime.AfterNextByte);

			nTransmitByte = (short) ((nDataCount - 1) & 0x1F);

			try
			{
				// Send the byte and start strong pullup
				owAdapter.SendByte(nTransmitByte);
			}
			catch
			{
				// Stop the strong pullup			
				owAdapter.SetLevel(TMEX.TMOneWireLevelOperation.Write, TMEX.TMOneWireLevelMode.Normal, TMEX.TMOneWireLevelPrime.Immediate);				

				// Re-throw the exception
				throw;
			}

			// Sleep while the data is transfered
			System.Threading.Thread.Sleep(6);

			// Stop the strong pullup
			nResult = owAdapter.SetLevel(TMEX.TMOneWireLevelOperation.Write, TMEX.TMOneWireLevelMode.Normal, TMEX.TMOneWireLevelPrime.Immediate);

			// Read data to see if the conversion is over
			nCheckByte = owAdapter.ReadByte();

			// Select and access the ID of the device we want to talk to
			owAdapter.Select(_deviceID);

			// Reinitialize the data count
			nDataCount = 0;

			// Set the read command into the transmit buffer
			aData[nDataCount++] = 0xAA;

			// Set the address to get the conversion results
			aData[nDataCount++] = 0x00;
			aData[nDataCount++] = 0x00;

			// Add 10 bytes to be read - 8 for the data and 2 for the CRC
			for (iIndex = 0; iIndex < 10; iIndex++)
				aData[nDataCount++] = 0xFF;

			// Send the block to the device
			nResult = owAdapter.SendBlock(aData, nDataCount);

			// Calculate the CRC of the transmitted data
			iCalculatedCRC = owCRC16.Calculate(aData, 0, 10);

			// Reconstruct the CRC sent by the device
			iSentCRC = aData[nDataCount - 1] << 8;
			iSentCRC |= aData[nDataCount - 2];
			iSentCRC ^= 0xFFFF;

			// If the CRC doesn't match then throw an exception
			if (iCalculatedCRC != iSentCRC)
			{
				// Throw a CRC exception
				throw new owException(owException.owExceptionFunction.CRC, _deviceID);
			}

			// Convert the data into a double
			for (iIndex = 3; iIndex < 11; iIndex += 2)
			{
				// Reconstruct the two bytes into the 16-bit values
				iVoltageReadout = ((aData[iIndex + 1] << 8) | aData[iIndex]) & 0x0000FFFF;

				// Figure out the percentage of the top voltage is present
				dVoltages[(iIndex - 3) / 2] = iVoltageReadout / 65535.0;

				// Apply the percentage to the maximum voltage range
				dVoltages[(iIndex - 3) / 2] *= ((m_aControl[(iIndex - 3) + 1] & 0x01) == 0x01 ? 5.12 : 2.56);
			}

			return dVoltages;
		}

		#endregion
	}
}
