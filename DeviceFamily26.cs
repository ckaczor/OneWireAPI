namespace OneWireAPI
{
    public class DeviceFamily26 : Device
    {
        public DeviceFamily26(Session session, short[] id)
            : base(session, id)
        {
            // Just call the base constructor
        }

        public enum VoltageType : short
        {
            Supply,
            Output
        }

        private double GetVoltage(VoltageType type)
        {
            short busy;

            // Select and access the ID of the device we want to talk to
            Adapter.Select(Id);

            // Data buffer to send over the network
            var data = new byte[30];

            // How many bytes of data to send
            short dataCount = 0;

            // Set the command to recall the status/configuration page to the scratchpad
            data[dataCount++] = 0xB8;

            // Set the page number to recall
            data[dataCount++] = 0x00;

            // Send the data block
            Adapter.SendBlock(data, dataCount);

            // Clear the data count
            dataCount = 0;

            // Access the device we want to talk to
            Adapter.Access();

            // Set the command to read the scratchpad
            data[dataCount++] = 0xBE;

            // Set the page number to read
            data[dataCount++] = 0x00;

            // Add 9 bytes to be read - 8 for the data and 1 for the CRC
            for (var index = 0; index < 9; index++)
                data[dataCount++] = 0xFF;

            // Send the data block
            Adapter.SendBlock(data, dataCount);

            // Calculate the CRC of the scratchpad data
            var crc = Crc8.Calculate(data, 2, 9);

            // If the CRC doesn't match then throw an exception
            if (crc != data[10])
            {
                // Throw a CRC exception
                throw new OneWireException(OneWireException.ExceptionFunction.Crc, Id);
            }

            // TODO - Check if we really need to change the input selector
            if (true)
            {
                // Access the device we want to talk to
                Adapter.Access();

                // Reset the data count
                dataCount = 0;

                // Set the command to write the scratchpad
                data[dataCount++] = 0x4E;

                // Set the page number to write
                data[dataCount++] = 0x00;

                // Set or clear the AD bit based on the type requested
                if (type == VoltageType.Supply)
                    data[dataCount++] = (byte) (data[2] | 0x08);
                else
                    data[dataCount++] = (byte) (data[2] & 0xF7);

                // Move the existing data down in the array
                for (var index = 0; index < 7; index++)
                    data[dataCount++] = data[index + 4];

                // Send the data block
                Adapter.SendBlock(data, dataCount);

                // Reset the data count
                dataCount = 0;

                // Access the device we want to talk to
                Adapter.Access();

                // Set the command to copy the scratchpad
                data[dataCount++] = 0x48;

                // Set the page number to copy to
                data[dataCount++] = 0x00;

                // Send the data block
                Adapter.SendBlock(data, dataCount);

                // Loop until the data copy is complete
                do
                {
                    busy = Adapter.ReadByte();
                }
                while (busy == 0);
            }

            // Access the device we want to talk to
            Adapter.Access();

            // Send the voltage conversion command
            Adapter.SendByte(0xB4);

            // Loop until conversion is complete
            do
            {
                busy = Adapter.ReadByte();
            }
            while (busy == 0);

            // Clear the data count
            dataCount = 0;

            // Set the command to recall the status/configuration page to the scratchpad
            data[dataCount++] = 0xB8;

            // Set the page number to recall
            data[dataCount++] = 0x00;

            // Access the device we want to talk to
            Adapter.Access();

            // Send the data block
            Adapter.SendBlock(data, dataCount);

            // Clear the data count
            dataCount = 0;

            // Access the device we want to talk to
            Adapter.Access();

            // Set the command to read the scratchpad
            data[dataCount++] = 0xBE;

            // Set the page number to read
            data[dataCount++] = 0x00;

            // Add 9 bytes to be read - 8 for the data and 1 for the CRC
            for (var index = 0; index < 9; index++)
                data[dataCount++] = 0xFF;

            // Send the data block
            Adapter.SendBlock(data, dataCount);

            // Calculate the CRC of the scratchpad data
            crc = Crc8.Calculate(data, 2, 9);

            // If the CRC doesn't match then throw an exception
            if (crc != data[10])
            {
                // Throw a CRC exception
                throw new OneWireException(OneWireException.ExceptionFunction.Crc, Id);
            }

            // Assemble the voltage data
            var dVoltage = (double) ((data[6] << 8) | data[5]);

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
            // Select and access the ID of the device we want to talk to
            Adapter.Select(Id);

            // Send the conversion command byte 
            Adapter.SendByte(0x44);

            // Sleep while the data is converted
            System.Threading.Thread.Sleep(10);

            // Access the device we want to talk to
            Adapter.Access();

            // Data buffer to send over the network
            var data = new byte[30];

            // How many bytes of data to send
            short dataCount = 0;

            // Set the command to recall the status/configuration page to the scratchpad
            data[dataCount++] = 0xB8;

            // Set the page number to recall
            data[dataCount++] = 0x00;

            // Send the data block
            Adapter.SendBlock(data, dataCount);

            // Clear the data count
            dataCount = 0;

            // Access the device we want to talk to
            Adapter.Access();

            // Set the command to read the scratchpad
            data[dataCount++] = 0xBE;

            // Set the page number to read
            data[dataCount++] = 0x00;

            // Add 9 bytes to be read - 8 for the data and 1 for the CRC
            for (var iIndex = 0; iIndex < 9; iIndex++)
                data[dataCount++] = 0xFF;

            // Send the data block
            Adapter.SendBlock(data, dataCount);

            // Calculate the CRC of the scratchpad data
            var crc = Crc8.Calculate(data, 2, 9);

            // If the CRC doesn't match then throw an exception
            if (crc != data[10])
            {
                // Throw a CRC exception
                throw new OneWireException(OneWireException.ExceptionFunction.Crc, Id);
            }

            // Get the two bytes of temperature data
            int temperatureLsb = data[3];
            int temperatureMsb = data[4];

            // Shift the data into the right order
            var temperature = ((temperatureMsb << 8) | temperatureLsb) >> 3;

            // Return the temperature
            return temperature * 0.03125F;
        }
    }
}
