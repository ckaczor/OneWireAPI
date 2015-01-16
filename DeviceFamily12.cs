namespace OneWireAPI
{
    public class DeviceFamily12 : Device
    {
        private const byte ChannelAccessCommand = 0xF5;
        private const byte WriteStatusCommand = 0x55;
        private const byte ReadStatusCommand = 0xAA;

        public DeviceFamily12(Session session, short[] id)
            : base(session, id)
        {
            // Just call the base constructor
        }

        public bool IsPowered(byte[] state)
        {
            return ((state[0] & 0x80) == 0x80);
        }

        public bool GetLevel(int channel, byte[] state)
        {
            if (channel == 0)
                return ((state[0] & 0x04) == 0x04);

            return ((state[0] & 0x08) == 0x08);
        }

        public bool GetLatchState(int channel, byte[] state)
        {
            if (channel == 0)
                return ((state[1] & 0x20) != 0x20);

            return ((state[1] & 0x40) != 0x40);
        }

        public void SetLatchState(int channel, bool latchState, byte[] state)
        {
            if (channel == 0)
            {
                state[1] &= 0xDF;

                if (!latchState)
                    state[1] = (byte) (state[1] | 0x20);
            }
            else
            {
                state[1] &= 0xBF;

                if (!latchState)
                    state[1] = (byte) (state[1] | 0x40);
            }
        }

        public byte[] ReadDevice()
        {
            // Select and access the ID of the device we want to talk to
            Adapter.Select(Id);

            // Data buffer to send over the network
            var data = new byte[30];

            // How many bytes of data to send
            short dataCount = 0;

            // Set the commmand to execute
            data[dataCount++] = ChannelAccessCommand;

            // Set the data
            data[dataCount++] = 0x55;
            data[dataCount++] = 0xFF;

            // Read the info, dummy data and CRC16
            for (var i = 3; i < 7; i++)
                data[dataCount++] = 0xFF;

            // Send the data
            Adapter.SendBlock(data, dataCount);

            // Calculate the CRC
            var crcResult = Crc16.Calculate(data, 0, 4);

            // Assemble the CRC provided by the device
            var matchCrc = data[6] << 8;
            matchCrc |= data[5];
            matchCrc ^= 0xFFFF;

            // Make sure the CRC values match
            if (crcResult != matchCrc)
            {
                // Throw a CRC exception
                throw new OneWireException(OneWireException.ExceptionFunction.Crc, Id);
            }

            var state = new byte[2];

            // Store the state data			
            state[0] = data[3];

            // Reset the data count
            dataCount = 0;

            // Set the command
            data[dataCount++] = ReadStatusCommand;

            // Set the address to read
            data[dataCount++] = 7;
            data[dataCount++] = 0;

            // Add data for the CRC
            for (var i = 3; i < 6; i++)
                data[dataCount++] = 0xFF;

            // Select and access the ID of the device we want to talk to
            Adapter.Select(Id);

            // Send the data
            Adapter.SendBlock(data, dataCount);

            // Calculate the CRC
            crcResult = Crc16.Calculate(data, 0, 3);

            // Assemble the CRC provided by the device
            matchCrc = data[5] << 8;
            matchCrc |= data[4];
            matchCrc ^= 0xFFFF;

            // Make sure the CRC values match
            if (crcResult != matchCrc)
            {
                // Throw a CRC exception
                throw new OneWireException(OneWireException.ExceptionFunction.Crc, Id);
            }

            // Store the state data
            state[1] = data[3];

            return state;
        }

        public void WriteDevice(byte[] state)
        {
            // Select and access the ID of the device we want to talk to
            Adapter.Select(Id);

            // Data buffer to send over the network
            var data = new byte[30];

            // How many bytes of data to send
            short dataCount = 0;

            // Set the commmand to execute
            data[dataCount++] = WriteStatusCommand;

            // Set the address
            data[dataCount++] = 0x07;
            data[dataCount++] = 0x00;

            // Add the state
            data[dataCount++] = state[1];

            // Add bytes for the CRC result
            data[dataCount++] = 0xFF;
            data[dataCount++] = 0xFF;

            // Send the data
            Adapter.SendBlock(data, dataCount);

            // Calculate the CRC
            var crcResult = Crc16.Calculate(data, 0, 3);

            // Assemble the CRC provided by the device
            var matchCrc = data[5] << 8;
            matchCrc |= data[4];
            matchCrc ^= 0xFFFF;

            // Make sure the CRC values match
            if (crcResult != matchCrc)
            {
                // Throw a CRC exception
                throw new OneWireException(OneWireException.ExceptionFunction.Crc, Id);
            }
        }
    }
}
