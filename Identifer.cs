using System.Collections.Generic;
using System.Text;

namespace OneWireAPI
{
    public class Identifier
    {
        public short[] RawId { get; private set; }

        public string Name { get; private set; }

        public int Family { get; private set; }

        public Identifier()
        {
            // Create a blank ID
            RawId = new short[8];
        }

        public Identifier(IList<byte> deviceId)
        {
            // Create a blank ID
            RawId = new short[8];

            // Copy the byte array to the short array
            for (var index = 0; index < deviceId.Count; index++)
                RawId[index] = deviceId[index];

            // Get the friendly name
            Name = ConvertToString(RawId);

            // Get the family code
            Family = RawId[0];
        }

        public Identifier(short[] deviceId)
        {
            // Store the ID supplied
            RawId = deviceId;

            // Get the friendly name
            Name = ConvertToString(RawId);

            // Get the family code
            Family = RawId[0];
        }

        private static string ConvertToString(IList<short> rawId)
        {
            var friendlyId = new StringBuilder();

            // Loop backwards over the ID array
            for (var index = rawId.Count - 1; index >= 0; index--)
            {
                // Convert the short value into a hex string and append it to the ID string
                friendlyId.AppendFormat("{0:X2}", rawId[index]);
            }

            // Return the ID string
            return friendlyId.ToString();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
