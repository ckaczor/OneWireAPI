namespace OneWireAPI
{
    public class DeviceFamily1D : Device
    {
        public DeviceFamily1D(Session session, short[] id)
            : base(session, id)
        {
            // Just call the base constructor
        }

        public uint GetCounter(int counterPage)
        {
            // Select and access the ID of the device we want to talk to
            Adapter.Select(DeviceId);

            // Data buffer to send over the network
            var data = new byte[30];

            // How many bytes of data to send
            short dataCount = 0;

            // Set the "read memory and counter" command into the data array
            data[dataCount++] = 0xA5;

            // Calculate the position of the last byte in the page
            var lastByte = (counterPage << 5) + 31;

            // Copy the lower byte of the last byte into the data array
            data[dataCount++] = (byte) (lastByte & 0xFF);

            // Copy the upper byte of the last byte into the data array
            data[dataCount++] = (byte) (lastByte >> 8);

            // Add byte for the data byate, counter, zero bits, and CRC16 result
            for (var i = 0; i < 11; i++) data[dataCount++] = 0xFF;

            // Send the block of data to the device
            Adapter.SendBlock(data, dataCount);

            // Calculate the CRC based on the data
            var crcResult = Crc16.Calculate(data, 0, 11);

            // Assemble the CRC provided by the device
            var matchCrc = data[13] << 8;
            matchCrc |= data[12];
            matchCrc ^= 0xFFFF;

            // Make sure the CRC values match
            if (crcResult != matchCrc)
            {
                // Throw a CRC exception
                throw new OneWireException(OneWireException.ExceptionFunction.Crc, DeviceId);
            }

            uint counter = 0;

            // Assemble the counter data from the bytes retrieved
            for (var i = dataCount - 7; i >= dataCount - 10; i--)
            {
                counter <<= 8;
                counter |= data[i];
            }

            return counter;
        }
    }
}
