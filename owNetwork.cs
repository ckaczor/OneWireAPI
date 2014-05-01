using System;
using System.Collections.Generic;

namespace OneWireAPI
{
    public class owNetwork 
    {
        #region Member variables

        private owSession _session;                             // Current session
        private Dictionary<string, owDevice> _deviceList;       // List of current devices

        #endregion

        #region Constructor

        public owNetwork(owSession Session)
        {
            _session = Session;

            _deviceList = new Dictionary<string, owDevice>();
        }

        #endregion

        #region	Delegates

        public delegate void DeviceEventDelegate(owDevice device);

        #endregion

        #region Events

        public event DeviceEventDelegate DeviceAdded;

        #endregion

        #region Private methods

        private void LoadDevices()
        {
            owDevice device;            // Current device object

            // Get the first device on the network
            short nResult = TMEX.TMFirst(_session.SessionHandle, _session.StateBuffer);

            // Keep looping while we get good device data
            while (nResult == 1)
            {
                // Create a new device ID buffer
                short[] nROM = new short[8];

                // Get the ROM from the device
                nResult = TMEX.TMRom(_session.SessionHandle, _session.StateBuffer, nROM);

                // If the ROM was read correctly then add the device to the list
                if (nResult == 1)
                {
                    // Get the deviceID
                    owIdentifier deviceID = new owIdentifier(nROM);

                    // Create a new device object
                    switch (deviceID.Family)
                    {
                        case 0x10:
                            device = new owDeviceFamily10(_session, nROM);
                            break;
                        case 0x1D:
                            device = new owDeviceFamily1D(_session, nROM);
                            break;
                        case 0x20:
                            device = new owDeviceFamily20(_session, nROM);
                            break;
                        case 0x26:
                            device = new owDeviceFamily26(_session, nROM);
                            break;
                        case 0x12:
                            device = new owDeviceFamily12(_session, nROM);
                            break;
                        case 0xFF:
                            device = new owDeviceFamilyFF(_session, nROM);
                            break;
                        default:
                            device = new owDevice(_session, nROM);
                            break;
                    }

                    // Check if we've seen this device before
                    if (!_deviceList.ContainsKey(device.ID.Name))
                    {
                        // Add the device to the device list
                        _deviceList[device.ID.Name] = device;

                        // Raise the device added event
                        if (DeviceAdded != null)
                            DeviceAdded(device);
                    }                    
                }

                // Try to get the next device
                nResult = TMEX.TMNext(_session.SessionHandle, _session.StateBuffer);
            }
        }

        #endregion

        #region Properties

        public Dictionary<string, owDevice> Devices
        {
            get { return _deviceList; }
        }

        #endregion

        #region Public methods

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

        #endregion
    }    
}
