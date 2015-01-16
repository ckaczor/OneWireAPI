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

        public Identifier DeviceId { get; private set; }

        public ExceptionFunction Function { get; private set; }

        public int Number { get; private set; }

        public OneWireException(ExceptionFunction function, int number)
        {
            // Store the exception function
            Function = function;

            // Store the exception number
            Number = number;
        }

        public OneWireException(ExceptionFunction function, Identifier deviceId)
        {
            // Store the exception function
            Function = function;

            // Store the device ID
            DeviceId = deviceId;
        }

        public OneWireException(ExceptionFunction function, Identifier deviceId, int number)
        {
            // Store the exception function
            Function = function;

            // Store the device ID
            DeviceId = deviceId;

            // Store the exception number
            Number = number;
        }

        public override string Message
        {
            get
            {
                switch (Function)
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
                        return "Unknown error in function" + Function;
                }
            }
        }
    }
}
