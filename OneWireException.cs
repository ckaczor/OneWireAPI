using System;

namespace OneWireAPI
{
    [Serializable]
    public class OneWireException : Exception
    {
        public enum ExceptionFunction
        {
            Access,
            Crc,
            ReadBit,
            ReadByte,
            Select,
            SendBit,
            SendBlock,
            SendByte,
            SetLevel
        }

        private readonly int _errorNumber;
        private readonly ExceptionFunction _errorFunction;
        private readonly Identifier _deviceId;

        public OneWireException(ExceptionFunction function, int number)
        {
            // Store the exception function
            _errorFunction = function;

            // Store the exception number
            _errorNumber = number;
        }

        public OneWireException(ExceptionFunction function, Identifier deviceId)
        {
            // Store the exception function
            _errorFunction = function;

            // Store the device ID
            _deviceId = deviceId;
        }

        public OneWireException(ExceptionFunction function, Identifier deviceId, int number)
        {
            // Store the exception function
            _errorFunction = function;

            // Store the device ID
            _deviceId = deviceId;

            // Store the exception number
            _errorNumber = number;
        }

        public Identifier DeviceId
        {
            get { return _deviceId; }
        }

        public ExceptionFunction Function
        {
            get { return _errorFunction; }
        }

        public override string Message
        {
            get
            {
                switch (_errorFunction)
                {
                    case ExceptionFunction.Access:
                        return "Unable to access device";
                    case ExceptionFunction.Crc:
                        return "CRC mismatch";
                    case ExceptionFunction.ReadBit:
                        return "Error reading bit";
                    case ExceptionFunction.ReadByte:
                        return "Error reading byte";
                    case ExceptionFunction.Select:
                        return "Unable to select device";
                    case ExceptionFunction.SendBit:
                        return "Error sending bit";
                    case ExceptionFunction.SendBlock:
                        return "Error sending block";
                    case ExceptionFunction.SendByte:
                        return "Error sending byte";
                    case ExceptionFunction.SetLevel:
                        return "Error setting level";
                    default:
                        return "Unknown error in function" + _errorFunction;
                }
            }
        }

        public int Number
        {
            get { return _errorNumber; }
        }
    }
}