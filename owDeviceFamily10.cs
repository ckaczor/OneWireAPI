using System;

namespace OneWireAPI
{
	public class owDeviceFamily10 : owDevice
	{
		#region Constructor

		public owDeviceFamily10(owSession Session, short[] ID) : base(Session, ID)
		{
			// Just call the base constructor
		}

		#endregion

		#region Methods

		public double GetTemperature()
		{
			short		nResult;								// Result of method calls
			byte[]		aData				= new byte[30];		// Data buffer to send over the network
			short		nDataCount			= 0;				// How many bytes of data to send
			double		dCountRemaining;						// How many counters remain in the temperature conversion
			double		dCountPerDegreeC;						// How many counters per degree C
			int			iTemperatureLSB;						// The LSB of the temperature
			double		dTemperature;							// double version of the temperature
			short		nCRC;									// Result of the CRC check

			// Select and access the ID of the device we want to talk to
			owAdapter.Select(_deviceID);

			// Setup for for power delivery after the next byte
			nResult = owAdapter.SetLevel(TMEX.TMOneWireLevelOperation.Write, TMEX.TMOneWireLevelMode.StrongPullup, TMEX.TMOneWireLevelPrime.AfterNextByte);

			try
			{
				// Send the byte and start strong pullup
				owAdapter.SendByte(0x44);
			}
			catch
			{
				// Stop the strong pullup			
				owAdapter.SetLevel(TMEX.TMOneWireLevelOperation.Write, TMEX.TMOneWireLevelMode.Normal, TMEX.TMOneWireLevelPrime.Immediate);

				// Re-throw the exception
				throw;
			}

			// Sleep while the data is transfered
			System.Threading.Thread.Sleep(1000);

			// Stop the strong pullup
			nResult = owAdapter.SetLevel(TMEX.TMOneWireLevelOperation.Write, TMEX.TMOneWireLevelMode.Normal, TMEX.TMOneWireLevelPrime.Immediate);

			// Access the device we want to talk to
			owAdapter.Access();

			// Set the command to get the temperature from the scatchpad
			aData[nDataCount++] = 0xBE;

			// Setup the rest of the bytes that we want
			for (int i = 0; i < 9; i++)
				aData[nDataCount++] = 0xFF;
		
			// Send the data block and get data back
			nResult = owAdapter.SendBlock(aData, nDataCount);

			// Calculate the CRC of the first eight bytes of data
			nCRC = owCRC8.Calculate(aData, 1, 8);

			// Check to see if our CRC matches the CRC supplied
			if (nCRC != aData[9])
			{
				// Throw a CRC exception
				throw new owException(owException.owExceptionFunction.CRC, _deviceID);
			}

			// Get the LSB of the temperature data and divide it by two
			iTemperatureLSB = aData[1] / 2;

			// If the data is negative then flip the bits
			if ((aData[2] & 0x01) == 0x01)	iTemperatureLSB |= -128;

			// Convert the temperature into a double
			dTemperature = (double) iTemperatureLSB;

			// Get the number of counts remaining
			dCountRemaining = aData[7];

			// Get the number of counts per degree C
			dCountPerDegreeC = aData[8];

			// Use the "counts remaining" data to calculate the temperaure to greater accuracy
			dTemperature = dTemperature - 0.25F + (dCountPerDegreeC - dCountRemaining) / dCountPerDegreeC;

			// Return the temperature
			return dTemperature;
		}

		#endregion
	}
}
