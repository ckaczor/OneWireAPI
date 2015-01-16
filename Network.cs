using System.Collections.Generic;

namespace OneWireAPI
{
    public class Network
    {
        private Session _session;                             // Current session
        private Dictionary<string, Device> _deviceList;       // List of current devices

        public Network(Session session)
        {
            _session = session;

            _deviceList = new Dictionary<string, Device>();
        }

        public delegate void DeviceEventDelegate(Device device);

        public event DeviceEventDelegate DeviceAdded;

        private void LoadDevices()
        {
            // Get the first device on the network
            var nResult = TMEX.TMFirst(_session.SessionHandle, _session.StateBuffer);

            // Keep looping while we get good device data
            while (nResult == 1)
            {
                // Create a new device ID buffer
                var id = new short[8];

                // Get the ROM from the device
                nResult = TMEX.TMRom(_session.SessionHandle, _session.StateBuffer, id);

                // If the ROM was read correctly then add the device to the list
                if (nResult == 1)
                {
                    // Get the deviceID
                    var deviceId = new Identifier(id);

                    // Create a new device object
                    Device device;

                    switch (deviceId.Family)
                    {
                        case 0x10:
                            device = new DeviceFamily10(_session, id);
                            break;
                        case 0x1D:
                            device = new DeviceFamily1D(_session, id);
                            break;
                        case 0x20:
                            device = new DeviceFamily20(_session, id);
                            break;
                        case 0x26:
                            device = new DeviceFamily26(_session, id);
                            break;
                        case 0x12:
                            device = new DeviceFamily12(_session, id);
                            break;
                        case 0xFF:
                            device = new DeviceFamilyFF(_session, id);
                            break;
                        default:
                            device = new Device(_session, id);
                            break;
                    }

                    // Check if we've seen this device before
                    if (!_deviceList.ContainsKey(device.Id.Name))
                    {
                        // Add the device to the device list
                        _deviceList[device.Id.Name] = device;

                        // Raise the device added event
                        if (DeviceAdded != null)
                            DeviceAdded(device);
                    }
                }

                // Try to get the next device
                nResult = TMEX.TMNext(_session.SessionHandle, _session.StateBuffer);
            }
        }

        public Dictionary<string, Device> Devices
        {
            get { return _deviceList; }
        }

        public void Initialize()
        {
            // Load the device list 
            LoadDevices();
        }

        public void Terminate()
        {
            // Get rid of the device list
            _deviceList.Clear();
            _deviceList = null;

            // Get rid of the session
            _session = null;
        }
    }
}
