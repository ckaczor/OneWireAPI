using Common.Debug;
using System;

namespace OneWireAPI
{
    public class Session
    {
        public short PortNumber { get; private set; }

        public short PortType { get; private set; }

        public int SessionHandle { get; private set; }

        public Network Network { get; private set; }

        public byte[] StateBuffer { get; private set; }

        public Session()
        {
            // Create the global state buffer
            StateBuffer = new byte[(int) TMEX.StateBufferSize.NoEpromWriting];

            short portNumber;
            short portType;

            // Get the default port number and type from the system
            var result = TMEX.TMReadDefaultPort(out portNumber, out portType);

            PortNumber = portNumber;
            PortType = portType;

            Tracer.WriteLine("TMReadDefaultPort - Return: {0}, Port Number: {1}, Port Type: {2}", result, PortNumber, PortType);
        }

        public Session(short portNumber, short portType)
        {
            // Create the global state buffer
            StateBuffer = new byte[(int) TMEX.StateBufferSize.NoEpromWriting];

            // Store the port number and type specified
            PortNumber = portNumber;
            PortType = portType;
        }

        public bool Acquire()
        {
            // Create a byte array to hold the version information
            var version = new byte[80];

            // Get the version 
            TMEX.Get_Version(version);

            // Decode the version
            var sVersion = System.Text.Encoding.Default.GetString(version, 0, version.Length);

            // Strip everything up to the first null character
            sVersion = sVersion.Substring(0, sVersion.IndexOf("\0", StringComparison.Ordinal));

            Tracer.WriteLine("Version: {0}", sVersion);

            Tracer.WriteLine("Starting Aquire");

            // Start a session on the port
            SessionHandle = TMEX.TMExtendedStartSession(PortNumber, PortType, IntPtr.Zero);

            Tracer.WriteLine("TMExtendedStartSession - Return: {0}", SessionHandle);

            // If we didn't get a session then throw an error
            if (SessionHandle <= 0)
                return false;

            // Setup the port for the current session
            var result = TMEX.TMSetup(SessionHandle);

            Tracer.WriteLine("TMSetup - Return: {0}", result);

            // Check the result
            if (result != 1)
            {
                // Release the session
                Release();

                return false;
            }

            // Create the network object and pass ourself as the session
            Network = new Network(this);

            // Initialize the network
            Network.Initialize();

            // Initialize the static adapter code with the session
            Adapter.Initialize(this);

            return true;
        }

        public void Release()
        {
            Tracer.WriteLine("Starting Release");

            // Terminate the network
            if (Network != null)
            {
                Network.Terminate();
                Network = null;
            }

            // Close the session
            var result = TMEX.TMClose(SessionHandle);

            Tracer.WriteLine("TMClose - Return: {0}", result);

            // End the session
            result = TMEX.TMEndSession(SessionHandle);

            Tracer.WriteLine("TMEndSession - Return: {0}", result);

            // Clear the session variable
            SessionHandle = 0;
        }
    }
}
