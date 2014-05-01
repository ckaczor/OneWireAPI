using System;
using System.Text;

namespace OneWireAPI
{
    public class owIdentifier 
    {
        #region Member variables

        private readonly short[] _rawID;                // The raw ID array
        private readonly string _friendlyName;          // Friendly display name
        private readonly int _familyCode;               // Family code

        #endregion

        #region Constructors

        public owIdentifier()
        {
            // Create a blank ID
            _rawID = new short[8];
        }

        public owIdentifier(byte[] deviceID)
        {
            // Create a blank ID
            _rawID = new short[8];

            // Copy the byte array to the short array
            for (int i = 0; i < deviceID.Length; i++) 
                _rawID[i] = deviceID[i];

            // Get the friendly name
            _friendlyName = ConvertToString(_rawID);

            // Get the family code
            _familyCode = _rawID[0];
        }

        public owIdentifier(short[] deviceID)
        {
            // Store the ID supplied
            _rawID = deviceID;

            // Get the friendly name
            _friendlyName = ConvertToString(_rawID);

            // Get the family code
            _familyCode = _rawID[0];
        }

        #endregion

        #region Private methods

        private static string ConvertToString(short[] rawID)
        {
            StringBuilder friendlyID = new StringBuilder();

            // Loop backwards over the ID array
            for (int iIndex = rawID.Length - 1; iIndex >= 0; iIndex--)
            {
                // Convert the short value into a hex string and append it to the ID string
                friendlyID.AppendFormat("{0:X2}", rawID[iIndex]);
            }

            // Return the ID string
            return friendlyID.ToString();
        }

        #endregion

        #region Properties

        public short[] RawID
        {
            get { return _rawID; }
        }

        public string Name
        {
            get { return _friendlyName; }
        }

        public int Family
        {
            get { return _familyCode; }
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return _friendlyName;
        }

        #endregion
    }
}
