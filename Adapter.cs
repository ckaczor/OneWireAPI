namespace OneWireAPI
{
    public class Adapter
    {
        private static Session _session;
        private static Identifier _lastId;

        public static void Initialize(Session session)
        {
            // Store the session we are dealing with
            _session = session;
        }

        public static void Select(Identifier id)
        {
            // Set the ID of the device we want to talk to
            var result = TMEX.TMRom(_session.SessionHandle, _session.StateBuffer, id.RawId);

            // Check the result
            if (result != 1)
            {
                // Throw a ROM exception
                throw new OneWireException(OneWireException.ExceptionFunction.Select, id, result);
            }

            // Copy the ID as the last selected ID
            _lastId = id;

            // Access the device
            Access();
        }

        public static void Access()
        {
            // Attempt to access the device
            var result = TMEX.TMAccess(_session.SessionHandle, _session.StateBuffer);

            // Check to see if we could access the device
            if (result != 1)
            {
                // Throw an access exception
                throw new OneWireException(OneWireException.ExceptionFunction.Access, _lastId, result);
            }
        }

        public static short SendBlock(byte[] data, short byteCount)
        {
            // Send the block and return the result
            var result = TMEX.TMBlockStream(_session.SessionHandle, data, byteCount);

            // Check to see if the bytes sent matches the value returned
            if (result != byteCount)
            {
                // Throw an access exception
                throw new OneWireException(OneWireException.ExceptionFunction.SendBlock, _lastId, result);
            }

            // Return the result
            return result;
        }

        public static short SendBlock(byte[] data, short byteCount, bool reset)
        {
            // Send the block and return the result
            var result = reset ? TMEX.TMBlockStream(_session.SessionHandle, data, byteCount) : TMEX.TMBlockIO(_session.SessionHandle, data, byteCount);

            // Check to see if the bytes sent matches the value returned
            if (result != byteCount)
            {
                // Throw an access exception
                throw new OneWireException(OneWireException.ExceptionFunction.SendBlock, _lastId, result);
            }

            // Return the result
            return result;
        }

        public static short ReadBit()
        {
            // Send the byte and get back what was sent
            var result = TMEX.TMTouchBit(_session.SessionHandle, 0xFF);

            // Return the result
            return result;
        }

        public static short SendBit(short output)
        {
            // Send the byte and get back what was sent
            var result = TMEX.TMTouchBit(_session.SessionHandle, output);

            // Check that the value was sent correctly
            if (result != output)
            {
                // Throw an exception
                throw new OneWireException(OneWireException.ExceptionFunction.SendBit, _lastId);
            }

            // Return the result
            return result;
        }

        public static short ReadByte()
        {
            // Send the byte and get back what was sent
            var result = TMEX.TMTouchByte(_session.SessionHandle, 0xFF);

            // Return the result
            return result;
        }

        public static short Reset()
        {
            // Reset all devices
            return TMEX.TMTouchReset(_session.SessionHandle);
        }

        public static short SendByte(short output)
        {
            // Send the byte and get back what was sent
            var result = TMEX.TMTouchByte(_session.SessionHandle, output);

            // Check that the value was sent correctly
            if (result != output)
            {
                // Throw an exception
                throw new OneWireException(OneWireException.ExceptionFunction.SendByte, _lastId);
            }

            // Return the result
            return result;
        }

        public static short SetLevel(TMEX.LevelOperation nOperation, TMEX.LevelMode nLevelMode, TMEX.LevelPrime nPrimed)
        {
            // Set the level
            var result = TMEX.TMOneWireLevel(_session.SessionHandle, TMEX.LevelOperation.Write, TMEX.LevelMode.Normal, TMEX.LevelPrime.Immediate);

            // Check the result
            if (result < 0)
            {
                // Throw an exception
                throw new OneWireException(OneWireException.ExceptionFunction.SetLevel, result);
            }

            // Return the result
            return result;
        }
    }
}
