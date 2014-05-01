using System;

namespace OneWireAPI
{
    public class owDevice 
    {
        #region Member variables

        protected owSession _session;               // The current session
        protected owIdentifier _deviceID;           // The ID of this device

        #endregion

        #region Constructor

        public owDevice(owSession session, short[] rawID)
        {
            // Store the session
            _session = session;

            // Create a new identifier and give it the ID supplied
            _deviceID = new owIdentifier(rawID);
        }

        #endregion

        #region Properties

        public owIdentifier ID
        {
            get { return _deviceID; }
        }

        public int Family
        {
            get { return _deviceID.Family; }
        }

        #endregion
    }
}
