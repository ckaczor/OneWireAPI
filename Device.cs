namespace OneWireAPI
{
    public class Device
    {
        protected Session Session;
        protected Identifier DeviceId;

        public Device(Session session, short[] rawId)
        {
            // Store the session
            Session = session;

            // Create a new identifier and give it the ID supplied
            DeviceId = new Identifier(rawId);
        }

        public Identifier Id
        {
            get { return DeviceId; }
        }

        public int Family
        {
            get { return DeviceId.Family; }
        }
    }
}
