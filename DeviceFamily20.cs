namespace OneWireAPI
{
    public class DeviceFamily20 : Device
    {
        private readonly byte[] _control = new byte[16];

        public enum Range : byte
        {
            Range512 = 0x01,
            Range256 = 0x00
        }

        public enum Resolution : byte
        {
            EightBits = 0x08,
            SixteenBits = 0x00
        }

        public DeviceFamily20(Session session, short[] id)
            : base(session, id)
        {
        }

        public void Initialize()
        {
            const int startAddress = 0x8;       // Starting data page address
            const int endAddress = 0x11;        // Ending data page address

            // Setup the control page
            for (var index = 0; index < 8; index += 2)
            {
                _control[index] = (byte) Resolution.EightBits;
                _control[index + 1] = (byte) Range.Range512;
            }

            // Clear the alarm page
            for (var index = 8; index < 16; index++)
            {
                _control[index] = 0;
            }

            // Data buffer to send over the network
            var data = new byte[30];

            // How many bytes of data to send
            short dataCount = 0;

            // Set the command into the data array
            data[dataCount++] = 0x55;

            // Set the starting address of the data to write
            data[dataCount++] = startAddress & 0xFF;
            data[dataCount++] = (startAddress >> 8) & 0xFF;

            // Select and access the ID of the device we want to talk to
            Adapter.Select(DeviceId);

            // Write to the data pages specified
            for (var index = startAddress; index <= endAddress; index++)
            {
                // Copy the control data into our output buffer
                data[dataCount++] = _control[index - startAddress];

                // Add two bytes for the CRC results
                data[dataCount++] = 0xFF;
                data[dataCount++] = 0xFF;

                // Add a byte for the control byte echo
                data[dataCount++] = 0xFF;

                // Send the block
                Adapter.SendBlock(data, dataCount);

                // If the check byte doesn't match then throw an exception
                if (data[dataCount - 1] != _control[index - startAddress])
                {
                    // Throw an exception
                    throw new OneWireException(OneWireException.ExceptionFunction.SendBlock, DeviceId);
                }

                int calculatedCrc;							// CRC we calculated from sent data
                int sentCrc;								// CRC retrieved from the device

                // Calculate the CRC values
                if (index == startAddress)
                {
                    // Calculate the CRC16 of the data sent
                    calculatedCrc = Crc16.Calculate(data, 0, 3);

                    // Reconstruct the CRC sent by the device
                    sentCrc = data[dataCount - 2] << 8;
                    sentCrc |= data[dataCount - 3];
                    sentCrc ^= 0xFFFF;
                }
                else
                {
                    // Calculate the CRC16 of the data sent
                    calculatedCrc = Crc16.Calculate(_control[index - startAddress], index);

                    // Reconstruct the CRC sent by the device
                    sentCrc = data[dataCount - 2] << 8;
                    sentCrc |= data[dataCount - 3];
                    sentCrc ^= 0xFFFF;
                }

                // If the CRC doesn't match then throw an exception
                if (calculatedCrc != sentCrc)
                {
                    // Throw a CRC exception
                    throw new OneWireException(OneWireException.ExceptionFunction.Crc, DeviceId);
                }

                // Reset the byte count
                dataCount = 0;
            }
        }

        public double[] GetVoltages()
        {
            // Select and access the ID of the device we want to talk to
            Adapter.Select(DeviceId);

            // Data buffer to send over the network
            var data = new byte[30];

            // How many bytes of data to send
            short dataCount = 0;

            // Set the convert command into the transmit buffer
            data[dataCount++] = 0x3C;

            // Set the input mask to get all channels
            data[dataCount++] = 0x0F;

            // Set the read-out control to leave things as they are
            data[dataCount++] = 0x00;

            // Add two bytes for the CRC results
            data[dataCount++] = 0xFF;
            data[dataCount++] = 0xFF;

            // Send the data block
            Adapter.SendBlock(data, dataCount);

            // Calculate the CRC based on the transmit buffer
            var calculatedCrc = Crc16.Calculate(data, 0, 2);

            // Reconstruct the CRC sent by the device
            var sentCrc = data[4] << 8;
            sentCrc |= data[3];
            sentCrc ^= 0xFFFF;

            // If the CRC doesn't match then throw an exception
            if (calculatedCrc != sentCrc)
            {
                // Throw a CRC exception
                throw new OneWireException(OneWireException.ExceptionFunction.Crc, DeviceId);
            }

            // Setup for for power delivery after the next byte
            Adapter.SetLevel(TMEX.LevelOperation.Write, TMEX.LevelMode.StrongPullup, TMEX.LevelPrime.AfterNextByte);

            var nTransmitByte = (short) ((dataCount - 1) & 0x1F);

            try
            {
                // Send the byte and start strong pullup
                Adapter.SendByte(nTransmitByte);
            }
            catch
            {
                // Stop the strong pullup			
                Adapter.SetLevel(TMEX.LevelOperation.Write, TMEX.LevelMode.Normal, TMEX.LevelPrime.Immediate);

                // Re-throw the exception
                throw;
            }

            // Sleep while the data is transfered
            System.Threading.Thread.Sleep(6);

            // Stop the strong pullup
            Adapter.SetLevel(TMEX.LevelOperation.Write, TMEX.LevelMode.Normal, TMEX.LevelPrime.Immediate);

            // Read data to see if the conversion is over
            Adapter.ReadByte();

            // Select and access the ID of the device we want to talk to
            Adapter.Select(DeviceId);

            // Reinitialize the data count
            dataCount = 0;

            // Set the read command into the transmit buffer
            data[dataCount++] = 0xAA;

            // Set the address to get the conversion results
            data[dataCount++] = 0x00;
            data[dataCount++] = 0x00;

            // Add 10 bytes to be read - 8 for the data and 2 for the CRC
            for (var index = 0; index < 10; index++)
                data[dataCount++] = 0xFF;

            // Send the block to the device
            Adapter.SendBlock(data, dataCount);

            // Calculate the CRC of the transmitted data
            calculatedCrc = Crc16.Calculate(data, 0, 10);

            // Reconstruct the CRC sent by the device
            sentCrc = data[dataCount - 1] << 8;
            sentCrc |= data[dataCount - 2];
            sentCrc ^= 0xFFFF;

            // If the CRC doesn't match then throw an exception
            if (calculatedCrc != sentCrc)
            {
                // Throw a CRC exception
                throw new OneWireException(OneWireException.ExceptionFunction.Crc, DeviceId);
            }

            // Voltage values to return
            var voltages = new double[4];

            // Convert the data into a double
            for (var index = 3; index < 11; index += 2)
            {
                // Reconstruct the two bytes into the 16-bit values
                var iVoltageReadout = ((data[index + 1] << 8) | data[index]) & 0x0000FFFF;

                // Figure out the percentage of the top voltage is present
                voltages[(index - 3) / 2] = iVoltageReadout / 65535.0;

                // Apply the percentage to the maximum voltage range
                voltages[(index - 3) / 2] *= ((_control[(index - 3) + 1] & 0x01) == 0x01 ? 5.12 : 2.56);
            }

            return voltages;
        }
    }
}
