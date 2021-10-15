using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Endpoints.Receivers;
using Crestron.SimplSharpPro.GeneralIO;
using Crestron.SimplSharpPro.UI;

namespace HuddleRoom
{
    public class ControlSystem : CrestronControlSystem
    {
        private BasicTriListWithSmartObject _tp;
        private CenOdtCPoe _occSensor;
        private DmRmc4kz100C _rmc;
        private ushort _src;

        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in ControlSystem: {0}", e.Message);
            }
        }

        public override void InitializeSystem()
        {
            try
            {
                _tp = new Tsw1060(0x03, this);
                _tp.OnlineStatusChange += tp_OnlineStatusChange;
                _tp.SigChange += tp_SigChange;

                if (_tp.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("Unable to register TSW-1060: {0}", _tp.RegistrationFailureReason);
                }

                _occSensor = new CenOdtCPoe(0x04, this);
                _occSensor.CenOccupancySensorChange += occSensor_CenOccupancySensorChange;

                if (_occSensor.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("Unable to register CEN-ODT-C-POE: {0}", _occSensor.RegistrationFailureReason);
                }

                _rmc = new DmRmc4kz100C(0x14, this);
                _rmc.ComPorts[1].SetComPortSpec(ComPort.eComBaudRates.ComspecBaudRate9600,
                    ComPort.eComDataBits.ComspecDataBits8, ComPort.eComParityType.ComspecParityNone,
                    ComPort.eComStopBits.ComspecStopBits1, ComPort.eComProtocolType.ComspecProtocolRS232,
                    ComPort.eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
                    ComPort.eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone, false);
                _rmc.ComPorts[1].SerialDataReceived += display_DataReceived;

                if (_rmc.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("Unable to register DM-RMC-4KZ-100-C: {0}", _rmc.RegistrationFailureReason);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        public void SetSource (ushort newSource)
        {
            _src = newSource;

            if (_src == 0)
            {
                system_Off();
            }
            else
            {
                system_On();
            }
        }

        private void tp_OnlineStatusChange(GenericBase dev, OnlineOfflineEventArgs args)
        {
            if (args.DeviceOnLine)
            {
                var tp = (BasicTriList)dev;

                tp.StringInput[1].StringValue = "Huddle Room";
                tp.StringInput[2].StringValue = "x1234";

                tp.UShortInput[1].UShortValue = _src;
            }
        }

        private void tp_SigChange(BasicTriList dev, SigEventArgs args)
        {
            switch (args.Sig.Type)
            {
                case eSigType.UShort:
                    switch (args.Sig.Number)
                    {
                        case 1:
                            SetSource(args.Sig.UShortValue);
                            break;
                    }
                    break;
            }
        }

        private void occSensor_CenOccupancySensorChange(object sender, GenericEventArgs args)
        {
            var sensor = sender as CenOdtCPoe;

            switch (args.EventId)
            {
                case GlsOccupancySensorBase.RoomOccupiedFeedbackEventId:
                    if (sensor.OccupancyDetectedFeedback.BoolValue)
                    {
                        SetSource(3);   // default to Room PC
                    }
                    break;
                case GlsOccupancySensorBase.RoomVacantFeedbackEventId:
                    if (sensor.VacancyDetectedFeedback.BoolValue)
                    {
                        SetSource(0);
                    }
                    break;
            }
        }

        private void display_DataReceived(ComPort port, ComPortSerialDataEventArgs args)
        {
            // TODO
        }

        public void system_On()
        {
            _rmc.ComPorts[1].Send("PWR ON\r");
        }

        public void system_Off()
        {
            _rmc.ComPorts[1].Send("PWR OFF\r");
        }
    }
}