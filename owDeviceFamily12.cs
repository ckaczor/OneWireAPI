using System;

namespace OneWireAPI
{
	public class owDeviceFamily12 : owDevice
	{
		#region Constants

		private const byte	CHANNEL_ACCESS_COMMAND		= 0xF5;			// Command value to access a channel
		private const byte	WRITE_STATUS_COMMAND		= 0x55;			// Command value to write the status
		private const byte	READ_STATUS_COMMAND			= 0xAA;			// Command value to read the status

		#endregion
		
		#region Constructor

		public owDeviceFamily12(owSession Session, short[] ID) : base(Session, ID)
		{
			// Just call the base constructor
		}

		#endregion

		#region Methods

		public bool IsPowered(byte[] State)
		{
			return ((State[0] & 0x80) == 0x80);
		}

		public bool GetLevel(int Channel, byte[] State)
		{
			if (Channel == 0)
				return ((State[0] & 0x04) == 0x04);
			else
				return ((State[0] & 0x08) == 0x08);
		}

		public bool GetLatchState(int Channel, byte[] State)
		{
			if (Channel == 0)
			{
				return ((State[1] & 0x20) != 0x20);
			}
			else
			{
				return ((State[1] & 0x40) != 0x40);
			}
		}

		public void SetLatchState(int Channel, bool LatchState, byte[] State)
		{
			if (Channel == 0)
			{
				State[1] &= 0xDF;
				
				if (!LatchState)	State[1] = (byte) (State[1] | 0x20);
			}
			else
			{
				State[1] &= 0xBF;
				
				if (!LatchState)	State[1] = (byte) (State[1] | 0x40);
			}
		}

		public byte[] ReadDevice()
		{
			byte[]		State				= new byte[2];
			byte[]		aData				= new byte[30];		// Data buffer to send over the network
			short		nDataCount			= 0;				// How many bytes of data to send
			int			iCRCResult;								// Result of the CRC calculation
			int			iMatchCRC;								// CRC retrieved from the device

			// Select and access the ID of the device we want to talk to
			owAdapter.Select(_deviceID);

			// Set the commmand to execute
			aData[nDataCount++] = CHANNEL_ACCESS_COMMAND;

			// Set the data
			aData[nDataCount++] = 0x55;
			aData[nDataCount++] = 0xFF;

			// Read the info, dummy data and CRC16
			for (int i = 3; i < 7; i++)
				aData[nDataCount++] = 0xFF;

			// Send the data
			owAdapter.SendBlock(aData, nDataCount);

			// Calculate the CRC
			iCRCResult = owCRC16.Calculate(aData, 0, 4);

			// Assemble the CRC provided by the device
			iMatchCRC = aData[6] << 8;
			iMatchCRC |= aData[5];
			iMatchCRC ^= 0xFFFF;

			// Make sure the CRC values match
			if (iCRCResult != iMatchCRC)
			{
				// Throw a CRC exception
				throw new owException(owException.owExceptionFunction.CRC, _deviceID);
			}

			// Store the state data			
			State[0] = aData[3];

			// Reset the data count
			nDataCount = 0;

			// Set the command
			aData[nDataCount++] = READ_STATUS_COMMAND;

			// Set the address to read
			aData[nDataCount++] = 7;
			aData[nDataCount++] = 0;

			// Add data for the CRC
			for (int i = 3; i < 6; i++)  
				aData[nDataCount++] = 0xFF;

			// Select and access the ID of the device we want to talk to
			owAdapter.Select(_deviceID);

			// Send the data
			owAdapter.SendBlock(aData, nDataCount);

			// Calculate the CRC
			iCRCResult = owCRC16.Calculate(aData, 0, 3);

			// Assemble the CRC provided by the device
			iMatchCRC = aData[5] << 8;
			iMatchCRC |= aData[4];
			iMatchCRC ^= 0xFFFF;

			// Make sure the CRC values match
			if (iCRCResult != iMatchCRC)
			{
				// Throw a CRC exception
				throw new owException(owException.owExceptionFunction.CRC, _deviceID);
			}

			// Store the state data
			State[1] = aData[3];

			return State;
		}

		public void WriteDevice(byte[] State)
		{
			short		nResult;								// Result of method calls
			byte[]		aData				= new byte[30];		// Data buffer to send over the network
			short		nDataCount			= 0;				// How many bytes of data to send
			int			iCRCResult;								// Result of the CRC calculation
			int			iMatchCRC;								// CRC retrieved from the device

			// Select and access the ID of the device we want to talk to
			owAdapter.Select(_deviceID);

			// Set the commmand to execute
			aData[nDataCount++] = WRITE_STATUS_COMMAND;

			// Set the address
			aData[nDataCount++] = 0x07;
			aData[nDataCount++] = 0x00;

			// Add the state
			aData[nDataCount++] = State[1];

			// Add bytes for the CRC result
			aData[nDataCount++] = 0xFF;
			aData[nDataCount++] = 0xFF;

			// Send the data
			nResult = owAdapter.SendBlock(aData, nDataCount);

			// Calculate the CRC
			iCRCResult = owCRC16.Calculate(aData, 0, 3);

			// Assemble the CRC provided by the device
			iMatchCRC = aData[5] << 8;
			iMatchCRC |= aData[4];
			iMatchCRC ^= 0xFFFF;

			// Make sure the CRC values match
			if (iCRCResult != iMatchCRC)
			{
				// Throw a CRC exception
				throw new owException(owException.owExceptionFunction.CRC, _deviceID);
			}

			return;
		}

		#endregion
	}
}
