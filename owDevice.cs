namespace OneWireAPI
{
    public class owDevice
    {
        protected owSession Session;
        protected owIdentifier DeviceId;

        public owDevice(owSession session, short[] rawId)
        {
            // Store the session
            Session = session;

            // Create a new identifier and give it the ID supplied
            DeviceId = new owIdentifier(rawId);
        }

        public owIdentifier Id
        {
            get { return DeviceId; }
        }

        public int Family
        {
            get { return DeviceId.Family; }
        }
    }
}
