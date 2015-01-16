namespace OneWireAPI
{
    public class DeviceFamily10 : Device
    {
        public DeviceFamily10(Session session, short[] id)
            : base(session, id)
        {
            // Just call the base constructor
        }

        public double GetTemperature()
        {
            // Select and access the ID of the device we want to talk to
            Adapter.Select(Id);

            // Setup for for power delivery after the next byte
            Adapter.SetLevel(TMEX.LevelOperation.Write, TMEX.LevelMode.StrongPullup, TMEX.LevelPrime.AfterNextByte);

            try
            {
                // Send the byte and start strong pullup
                Adapter.SendByte(0x44);
            }
            catch
            {
                // Stop the strong pullup			
                Adapter.SetLevel(TMEX.LevelOperation.Write, TMEX.LevelMode.Normal, TMEX.LevelPrime.Immediate);

                // Re-throw the exception
                throw;
            }

            // Sleep while the data is transfered
            System.Threading.Thread.Sleep(1000);

            // Stop the strong pullup
            Adapter.SetLevel(TMEX.LevelOperation.Write, TMEX.LevelMode.Normal, TMEX.LevelPrime.Immediate);

            // Access the device we want to talk to
            Adapter.Access();

            // Data buffer to send over the network
            var data = new byte[30];

            // How many bytes of data to send
            short dataCount = 0;

            // Set the command to get the temperature from the scatchpad
            data[dataCount++] = 0xBE;

            // Setup the rest of the bytes that we want
            for (var i = 0; i < 9; i++)
                data[dataCount++] = 0xFF;

            // Send the data block and get data back
            Adapter.SendBlock(data, dataCount);

            // Calculate the CRC of the first eight bytes of data
            var crc = Crc8.Calculate(data, 1, 8);

            // Check to see if our CRC matches the CRC supplied
            if (crc != data[9])
            {
                // Throw a CRC exception
                throw new OneWireException(OneWireException.ExceptionFunction.Crc, Id);
            }

            // Get the LSB of the temperature data and divide it by two
            var temperatureLsb = data[1] / 2;

            // If the data is negative then flip the bits
            if ((data[2] & 0x01) == 0x01) temperatureLsb |= -128;

            // Convert the temperature into a double
            var temperature = (double) temperatureLsb;

            // Get the number of counts remaining
            double countRemaining = data[7];

            // Get the number of counts per degree C
            double countPerDegreeC = data[8];

            // Use the "counts remaining" data to calculate the temperaure to greater accuracy
            temperature = temperature - 0.25F + (countPerDegreeC - countRemaining) / countPerDegreeC;

            // Return the temperature
            return temperature;
        }
    }
}
