namespace OneWireAPI
{
    public class Device
    {
        protected Session Session;

        public Identifier Id { get; protected set; }

        public Device(Session session, short[] rawId)
        {
            // Store the session
            Session = session;

            // Create a new identifier and give it the ID supplied
            Id = new Identifier(rawId);
        }

        public int Family
        {
            get { return Id.Family; }
        }
    }
}
