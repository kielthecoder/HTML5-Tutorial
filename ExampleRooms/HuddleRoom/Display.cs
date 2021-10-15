using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;

namespace HuddleRoom
{
    class Display
    {
        private ComPort _port;

        public Display(ComPort port)
        {
            _port = port;
            _port.SetComPortSpec(ComPort.eComBaudRates.ComspecBaudRate9600,
                    ComPort.eComDataBits.ComspecDataBits8, ComPort.eComParityType.ComspecParityNone,
                    ComPort.eComStopBits.ComspecStopBits1, ComPort.eComProtocolType.ComspecProtocolRS232,
                    ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                    ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone, false);
            _port.SerialDataReceived += _port_DataReceived;
        }

        public Display(ComPort port, ComPort.ComPortSpec spec)
        {
            _port = port;
            _port.SetComPortSpec(spec);
            _port.SerialDataReceived += _port_DataReceived;
        }

        public void TurnOn()
        {
            _port.Send("PWR ON\r");
        }

        public void TurnOff()
        {
            _port.Send("PWR OFF\r");
        }

        private void _port_DataReceived(ComPort port, ComPortSerialDataEventArgs args)
        {
            // TODO
        }
    }
}
