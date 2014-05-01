using System;

namespace OneWireAPI
{
	public class owDeviceFamilyFF : owDevice
	{
		#region Member variables

		private int	m_iWidth		=	20;
		private int	m_iHeight		=	4;

		#endregion

		#region Constructor

		public owDeviceFamilyFF(owSession Session, short[] ID) : base(Session, ID)
		{
			// Just call the base constructor
		}

		#endregion

		#region Methods

		public void SetBackLight(bool State)
		{
			// Select the device
			owAdapter.Select(_deviceID);

			// Set the state of the backlight
			owAdapter.SendByte((short) (State ? 0x8 : 0x7));
		}

		public void SetText(string Text)
		{
			string[]	sLines		= null;		// Array of lines
			int			iLine		= 1;		// Line number

            // Replace any CRLF pairs with just a newline
            Text = Text.Replace("\r\n", "\n");
			
			// Split the input string at any newlines
			sLines = Text.Split("\n".ToCharArray(), m_iHeight);

			// Loop over each line
			foreach (string sLine in sLines)
			{
				// Set the text of this line
				SetText(sLine, iLine++);
			}
		}

		public void SetText(string Text, int Line)
		{
			short	nMemoryPosition		= 0x00;			// Position at which to write the new string data
			string	sSendData			= "";			// String data to send
			byte[]	baData;								// Byte array of data to send
			short	nDataCount			= 0;			// Amount of data to send

			// Figure out the initial memory position based on the line number
			switch (Line)
			{
				case 1:
					nMemoryPosition = 0x00;
					break;

				case 2:
					nMemoryPosition = 0x40;
					break;

				case 3:
					nMemoryPosition = 0x14;
					break;

				case 4:
					nMemoryPosition = 0x54;
					break;
			}

			// Pad the text to the right width
			Text = Text.PadRight(m_iWidth);

			// The scratchpad is only 16 bytes long so we need to split it up
			if (m_iWidth > 16)
			{
				// Select the device
				owAdapter.Select(_deviceID);

				// Set the data block to just the first 16 characters
				sSendData = Text.Substring(0, 16);

				// Initialize the data array
				baData = new byte[18];

				// Set the command to write to the scratchpad 
				baData[nDataCount++] = 0x4E;

				// Set the memory position
				baData[nDataCount++] = (byte) nMemoryPosition;

				// Add the text data to the data
				foreach (byte bChar in System.Text.Encoding.Default.GetBytes(sSendData))
					baData[nDataCount++] = bChar;

				// Set the block
				owAdapter.SendBlock(baData, nDataCount);

				// Select the device
				owAdapter.Select(_deviceID);

				// Send the scratchpad data to the LCD
				owAdapter.SendByte(0x48);

				// Reset the device
				owAdapter.Reset();

				// Increment the memory position
				nMemoryPosition += 16;

				// Set the data to the rest of the line
				sSendData = Text.Substring(16, m_iWidth - 16);
			}
			else
			{
				// Just set the data string to whatever was passed in
				sSendData = Text;
			}

			// Select the device
			owAdapter.Select(_deviceID);

			// Initialize the data array
			baData = new byte[18];

			// Reset the data count
			nDataCount = 0;

			// Set the command to write to the scratchpad 
			baData[nDataCount++] = 0x4E;

			// Set the memory position
			baData[nDataCount++] = (byte) nMemoryPosition;

			// Add the text data to the data
			foreach (byte bChar in System.Text.Encoding.Default.GetBytes(sSendData))
				baData[nDataCount++] = bChar;

			// Set the block
			owAdapter.SendBlock(baData, nDataCount);

			// Select the device
			owAdapter.Select(_deviceID);

			// Send the scratchpad data to the LCD
			owAdapter.SendByte(0x48);

			// Reset the device
			owAdapter.Reset();
		}

		public void Clear()
		{
			// Select the device
			owAdapter.Select(_deviceID);

			// Clear the display
			owAdapter.SendByte(0x49);
		}

		#endregion
	}
}
