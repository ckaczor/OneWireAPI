using System;

namespace OneWireAPI
{
	public class owDeviceFamily26 : owDevice
	{
		#region Constructor

		public owDeviceFamily26(owSession Session, short[] ID) : base(Session, ID)
		{
			// Just call the base constructor
		}

		#endregion

		#region Methods

		public enum VoltageType : short
		{
			Supply,
			Output
		}

		private double GetVoltage(VoltageType Type)
		{
			short		nResult;								// Result of method calls
			byte[]		aData				= new byte[30];		// Data buffer to send over the network
			short		nDataCount			= 0;				// How many bytes of data to send
			short		nCRC;									// Result of the CRC check
			int			iIndex;
			short		nBusy;
			double		dVoltage;

			// Select and access the ID of the device we want to talk to
			owAdapter.Select(_deviceID);

			// Set the command to recall the status/configuration page to the scratchpad
			aData[nDataCount++] = 0xB8;

			// Set the page number to recall
			aData[nDataCount++] = 0x00;

			// Send the data block
			nResult = owAdapter.SendBlock(aData, nDataCount);

			// Clear the data count
			nDataCount = 0;

			// Access the device we want to talk to
			owAdapter.Access();

			// Set the command to read the scratchpad
			aData[nDataCount++] = 0xBE;

			// Set the page number to read
			aData[nDataCount++] = 0x00;

			// Add 9 bytes to be read - 8 for the data and 1 for the CRC
			for (iIndex = 0; iIndex < 9; iIndex++)
				aData[nDataCount++] = 0xFF;

			// Send the data block
			nResult = owAdapter.SendBlock(aData, nDataCount);

			// Calculate the CRC of the scratchpad data
			nCRC = owCRC8.Calculate(aData, 2, 9);

			// If the CRC doesn't match then throw an exception
			if (nCRC != aData[10])
			{
				// Throw a CRC exception
				throw new owException(owException.owExceptionFunction.CRC, _deviceID);
			}			

			// TODO - Check if we really need to change the input selector
			if (true)
			{
				// Access the device we want to talk to
				owAdapter.Access();

				// Reset the data count
                nDataCount = 0;

				// Set the command to write the scratchpad
				aData[nDataCount++] = 0x4E;

				// Set the page number to write
				aData[nDataCount++] = 0x00;

				// Set or clear the AD bit based on the type requested
				if (Type == VoltageType.Supply)
					aData[nDataCount++] = (byte) (aData[2] | 0x08);
				else
					aData[nDataCount++] = (byte) (aData[2] & 0xF7);

				// Move the existing data down in the array
				for (iIndex = 0; iIndex < 7; iIndex++)
					aData[nDataCount++] = aData[iIndex + 4];

				// Send the data block
				nResult = owAdapter.SendBlock(aData, nDataCount);

				// Reset the data count
				nDataCount = 0;
				
				// Access the device we want to talk to
				owAdapter.Access();

				// Set the command to copy the scratchpad
				aData[nDataCount++] = 0x48;

				// Set the page number to copy to
				aData[nDataCount++] = 0x00;

				// Send the data block
				nResult = owAdapter.SendBlock(aData, nDataCount);

				// Loop until the data copy is complete
				do
				{
					nBusy = owAdapter.ReadByte();
				}
				while (nBusy == 0);
			}

			// Access the device we want to talk to
			owAdapter.Access();

			// Send the voltage conversion command
			owAdapter.SendByte(0xB4);

			// Loop until conversion is complete
			do
			{
				nBusy = owAdapter.ReadByte();
			}
			while (nBusy == 0);

			// Clear the data count
			nDataCount = 0;

			// Set the command to recall the status/configuration page to the scratchpad
			aData[nDataCount++] = 0xB8;

			// Set the page number to recall
			aData[nDataCount++] = 0x00;

			// Access the device we want to talk to
			owAdapter.Access();

			// Send the data block
			nResult = owAdapter.SendBlock(aData, nDataCount);

			// Clear the data count
			nDataCount = 0;

			// Access the device we want to talk to
			owAdapter.Access();

			// Set the command to read the scratchpad
			aData[nDataCount++] = 0xBE;

			// Set the page number to read
			aData[nDataCount++] = 0x00;

			// Add 9 bytes to be read - 8 for the data and 1 for the CRC
			for (iIndex = 0; iIndex < 9; iIndex++)
				aData[nDataCount++] = 0xFF;

			// Send the data block
			nResult = owAdapter.SendBlock(aData, nDataCount);

			// Calculate the CRC of the scratchpad data
			nCRC = owCRC8.Calculate(aData, 2, 9);

			// If the CRC doesn't match then throw an exception
			if (nCRC != aData[10])
			{
				// Throw a CRC exception
				throw new owException(owException.owExceptionFunction.CRC, _deviceID);
			}			
			
			// Assemble the voltage data
			dVoltage = (double) ((aData[6] << 8) | aData[5]);
		
			return dVoltage / 100;
		}

		public double GetSupplyVoltage()
		{
			return GetVoltage(VoltageType.Supply);
		}

		public double GetOutputVoltage()
		{

			return GetVoltage(VoltageType.Output);
		}

		public double GetTemperature()
		{
			short		nResult;								// Result of method calls
			byte[]		aData				= new byte[30];		// Data buffer to send over the network
			short		nDataCount			= 0;				// How many bytes of data to send
			short		nCRC;									// Result of the CRC check
			int			iTemperatureLSB;						// The LSB of the temperature data 
			int			iTemperatureMSB;						// The MSB of the temperature data
			int			iTemperature;							// Complete temperature data
			double		dTemperature;							// double version of the temperature

			// Select and access the ID of the device we want to talk to
			owAdapter.Select(_deviceID);

			// Send the conversion command byte 
			owAdapter.SendByte(0x44);

			// Sleep while the data is converted
			System.Threading.Thread.Sleep(10);

			// Access the device we want to talk to
			owAdapter.Access();

			// Set the command to recall the status/configuration page to the scratchpad
			aData[nDataCount++] = 0xB8;

			// Set the page number to recall
			aData[nDataCount++] = 0x00;

			// Send the data block
			nResult = owAdapter.SendBlock(aData, nDataCount);

			// Clear the data count
			nDataCount = 0;

			// Access the device we want to talk to
			owAdapter.Access();

			// Set the command to read the scratchpad
			aData[nDataCount++] = 0xBE;

			// Set the page number to read
			aData[nDataCount++] = 0x00;

			// Add 9 bytes to be read - 8 for the data and 1 for the CRC
			for (int iIndex = 0; iIndex < 9; iIndex++)
				aData[nDataCount++] = 0xFF;

			// Send the data block
			nResult = owAdapter.SendBlock(aData, nDataCount);

			// Calculate the CRC of the scratchpad data
			nCRC = owCRC8.Calculate(aData, 2, 9);

			// If the CRC doesn't match then throw an exception
			if (nCRC != aData[10])
			{
				// Throw a CRC exception
				throw new owException(owException.owExceptionFunction.CRC, _deviceID);
			}			

			// Get the two bytes of temperature data
			iTemperatureLSB = aData[3];
			iTemperatureMSB = aData[4];

			// Shift the data into the right order
			iTemperature = ((iTemperatureMSB << 8) | iTemperatureLSB) >> 3;

			// Figure out the temperature 
			dTemperature = iTemperature * 0.03125F;

			// Return the temperature
			return dTemperature;
		}

		#endregion
	}
}
