using System.Collections.Generic;

namespace OneWireAPI
{
    public class owNetwork 
    {
        private owSession _session;                             // Current session
        private Dictionary<string, owDevice> _deviceList;       // List of current devices

        public owNetwork(owSession session)
        {
            _session = session;

            _deviceList = new Dictionary<string, owDevice>();
        }

        public delegate void DeviceEventDelegate(owDevice device);

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
                    var deviceId = new owIdentifier(id);

                    // Create a new device object
                    owDevice device;
              
                    switch (deviceId.Family)
                    {
                        case 0x10:
                            device = new owDeviceFamily10(_session, id);
                            break;
                        case 0x1D:
                            device = new owDeviceFamily1D(_session, id);
                            break;
                        case 0x20:
                            device = new owDeviceFamily20(_session, id);
                            break;
                        case 0x26:
                            device = new owDeviceFamily26(_session, id);
                            break;
                        case 0x12:
                            device = new owDeviceFamily12(_session, id);
                            break;
                        case 0xFF:
                            device = new owDeviceFamilyFF(_session, id);
                            break;
                        default:
                            device = new owDevice(_session, id);
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

        public Dictionary<string, owDevice> Devices
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
