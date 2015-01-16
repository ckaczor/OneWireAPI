namespace OneWireAPI
{
    public class owDeviceFamilyFF : owDevice
    {
        private int _width = 20;
        private int _height = 4;

        public owDeviceFamilyFF(owSession session, short[] id)
            : base(session, id)
        {
            // Just call the base constructor
        }

        public void SetSize(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void SetBackLight(bool state)
        {
            // Select the device
            owAdapter.Select(DeviceId);

            // Set the state of the backlight
            owAdapter.SendByte((short) (state ? 0x8 : 0x7));
        }

        public void SetText(string text)
        {
            // Line number
            var line = 1;

            // Replace any CRLF pairs with just a newline
            text = text.Replace("\r\n", "\n");

            // Split the input string at any newlines
            var sLines = text.Split("\n".ToCharArray(), _height);

            // Loop over each line
            foreach (var sLine in sLines)
            {
                // Set the text of this line
                SetText(sLine, line++);
            }
        }

        public void SetText(string text, int line)
        {
            // Position at which to write the new string data
            short memoryPosition = 0x00;

            // String data to send
            string sendData;

            // Byte array of data to send
            byte[] data;

            // Amount of data to send
            short dataCount = 0;

            // Figure out the initial memory position based on the line number
            switch (line)
            {
                case 1:
                    memoryPosition = 0x00;
                    break;

                case 2:
                    memoryPosition = 0x40;
                    break;

                case 3:
                    memoryPosition = 0x14;
                    break;

                case 4:
                    memoryPosition = 0x54;
                    break;
            }

            // Pad the text to the right width
            text = text.PadRight(_width);

            // The scratchpad is only 16 bytes long so we need to split it up
            if (_width > 16)
            {
                // Select the device
                owAdapter.Select(DeviceId);

                // Set the data block to just the first 16 characters
                sendData = text.Substring(0, 16);

                // Initialize the data array
                data = new byte[18];

                // Set the command to write to the scratchpad 
                data[dataCount++] = 0x4E;

                // Set the memory position
                data[dataCount++] = (byte) memoryPosition;

                // Add the text data to the data
                foreach (var bChar in System.Text.Encoding.Default.GetBytes(sendData))
                    data[dataCount++] = bChar;

                // Set the block
                owAdapter.SendBlock(data, dataCount);

                // Select the device
                owAdapter.Select(DeviceId);

                // Send the scratchpad data to the LCD
                owAdapter.SendByte(0x48);

                // Reset the device
                owAdapter.Reset();

                // Increment the memory position
                memoryPosition += 16;

                // Set the data to the rest of the line
                sendData = text.Substring(16, _width - 16);
            }
            else
            {
                // Just set the data string to whatever was passed in
                sendData = text;
            }

            // Select the device
            owAdapter.Select(DeviceId);

            // Initialize the data array
            data = new byte[18];

            // Reset the data count
            dataCount = 0;

            // Set the command to write to the scratchpad 
            data[dataCount++] = 0x4E;

            // Set the memory position
            data[dataCount++] = (byte) memoryPosition;

            // Add the text data to the data
            foreach (var bChar in System.Text.Encoding.Default.GetBytes(sendData))
                data[dataCount++] = bChar;

            // Set the block
            owAdapter.SendBlock(data, dataCount);

            // Select the device
            owAdapter.Select(DeviceId);

            // Send the scratchpad data to the LCD
            owAdapter.SendByte(0x48);

            // Reset the device
            owAdapter.Reset();
        }

        public void Clear()
        {
            // Select the device
            owAdapter.Select(DeviceId);

            // Clear the display
            owAdapter.SendByte(0x49);
        }
    }
}
