using System;

namespace OneWireAPI
{
    [Serializable]
    public class owException : Exception
    {
        #region Enumerations

        public enum owExceptionFunction
        {
            Access,
            CRC,
            ReadBit,
            ReadByte,
            Select,
            SendBit,
            SendBlock,
            SendByte,
            SetLevel
        }

        #endregion

        #region Private member variables

        private readonly int _errorNumber;
        private readonly owExceptionFunction _errorFunction;
        private readonly owIdentifier _deviceID;

        #endregion

        #region Constructors

        public owException(owExceptionFunction function, int number)
        {
            // Store the exception function
            _errorFunction = function;

            // Store the exception number
            _errorNumber = number;
        }

        public owException(owExceptionFunction function, owIdentifier deviceID)
        {
            // Store the exception function
            _errorFunction = function;

            // Store the device ID
            _deviceID = deviceID;
        }

        public owException(owExceptionFunction function, owIdentifier deviceID, int number)
        {
            // Store the exception function
            _errorFunction = function;

            // Store the device ID
            _deviceID = deviceID;

            // Store the exception number
            _errorNumber = number;
        }

        #endregion

        #region Properties

        public owIdentifier DeviceID
        {
            get { return _deviceID; }
        }

        public owExceptionFunction Function
        {
            get { return _errorFunction; }
        }

        public override string Message
        {
            get
            {
                switch (_errorFunction)
                {
                    case owExceptionFunction.Access:        return "Unable to access device";
                    case owExceptionFunction.CRC:           return "CRC mismatch";
                    case owExceptionFunction.ReadBit:       return "Error reading bit";
                    case owExceptionFunction.ReadByte:      return "Error reading byte";
                    case owExceptionFunction.Select:        return "Unable to select device";
                    case owExceptionFunction.SendBit:       return "Error sending bit";
                    case owExceptionFunction.SendBlock:     return "Error sending block";
                    case owExceptionFunction.SendByte:      return "Error sending byte";
                    case owExceptionFunction.SetLevel:      return "Error setting level";
                    default:                                return "Unknown error in function" + _errorFunction;
                }
            }
        }

        public int Number
        {
            get { return _errorNumber; }
        }

        #endregion
    }
}
