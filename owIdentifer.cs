using System.Text;

namespace OneWireAPI
{
    public class owIdentifier
    {
        private readonly short[] _rawId;                // The raw ID array
        private readonly string _friendlyName;          // Friendly display name
        private readonly int _familyCode;               // Family code

        public owIdentifier()
        {
            // Create a blank ID
            _rawId = new short[8];
        }

        public owIdentifier(byte[] deviceId)
        {
            // Create a blank ID
            _rawId = new short[8];

            // Copy the byte array to the short array
            for (var i = 0; i < deviceId.Length; i++)
                _rawId[i] = deviceId[i];

            // Get the friendly name
            _friendlyName = ConvertToString(_rawId);

            // Get the family code
            _familyCode = _rawId[0];
        }

        public owIdentifier(short[] deviceId)
        {
            // Store the ID supplied
            _rawId = deviceId;

            // Get the friendly name
            _friendlyName = ConvertToString(_rawId);

            // Get the family code
            _familyCode = _rawId[0];
        }

        private static string ConvertToString(short[] rawId)
        {
            var friendlyId = new StringBuilder();

            // Loop backwards over the ID array
            for (var iIndex = rawId.Length - 1; iIndex >= 0; iIndex--)
            {
                // Convert the short value into a hex string and append it to the ID string
                friendlyId.AppendFormat("{0:X2}", rawId[iIndex]);
            }

            // Return the ID string
            return friendlyId.ToString();
        }

        public short[] RawId
        {
            get { return _rawId; }
        }

        public string Name
        {
            get { return _friendlyName; }
        }

        public int Family
        {
            get { return _familyCode; }
        }

        public override string ToString()
        {
            return _friendlyName;
        }
    }
}
