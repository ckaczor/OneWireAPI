using Common.Debug;
using System;

namespace OneWireAPI
{
    public class owSession 
    {
        private int _sessionHandle;                 // Session handle
        private owNetwork _network;                 // Network object
        
        private readonly short _portNumber;         // Port number
        private readonly short _portType;           // Port type
        private readonly byte[] _stateBuffer;       // Global state buffer

        public owSession()
        {
            // Create the global state buffer
            _stateBuffer = new byte[(int) TMEX.StateBufferSize.NoEpromWriting];

            // Get the default port number and type from the system
            var result = TMEX.TMReadDefaultPort(out _portNumber, out _portType);

            Tracer.WriteLine("TMReadDefaultPort - Return: {0}, Port Number: {1}, Port Type: {2}", result, _portNumber, _portType);
        }

        public owSession(short portNumber, short portType)
        {
            // Create the global state buffer
            _stateBuffer = new byte[(int) TMEX.StateBufferSize.NoEpromWriting];

            // Store the port number and type specified
            _portNumber = portNumber;
            _portType = portType;
        }

        public short PortNumber
        {
            get { return _portNumber; }
        }

        public short PortType
        {
            get { return _portType; }
        }

        public int SessionHandle
        {
            get { return _sessionHandle; }
        }

        public owNetwork Network
        {
            get { return _network; }
        }

        public byte[] StateBuffer
        {
            get { return _stateBuffer; }
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
            _sessionHandle = TMEX.TMExtendedStartSession(_portNumber, _portType, IntPtr.Zero);

            Tracer.WriteLine("TMExtendedStartSession - Return: {0}", _sessionHandle);

            // If we didn't get a session then throw an error
            if (_sessionHandle <= 0)
                return false;

            // Setup the port for the current session
            var result = TMEX.TMSetup(_sessionHandle);

            Tracer.WriteLine("TMSetup - Return: {0}", result);

            // Check the result
            if (result != 1)
            {
                // Release the session
                Release();

                return false;
            }

            // Create the network object and pass ourself as the session
            _network = new owNetwork(this);

            // Initialize the network
            _network.Initialize();

            // Initialize the static adapter code with the session
            owAdapter.Initialize(this);

            return true;
        }

        public void Release()
        {
            Tracer.WriteLine("Starting Release");

            // Terminate the network
            if (_network != null)
            {
                _network.Terminate();
                _network = null;
            }

            // Close the session
            var result = TMEX.TMClose(_sessionHandle);

            Tracer.WriteLine("TMClose - Return: {0}", result);

            // End the session
            result = TMEX.TMEndSession(_sessionHandle);

            Tracer.WriteLine("TMEndSession - Return: {0}", result);

            // Clear the session variable
            _sessionHandle = 0;
        }
    }
}
