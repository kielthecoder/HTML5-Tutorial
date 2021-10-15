using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Endpoints.Receivers;
using Crestron.SimplSharpPro.DM.Endpoints.Transmitters;
using Crestron.SimplSharpPro.UI;

namespace HuddleRoom
{
    public enum SourceIds
    {
        None = 0,
        Laptop = 1,
        AppleTV = 2,
        RoomPC = 3
    }

    public class ControlSystem : CrestronControlSystem
    {
        private DmRmc4kz100C _rmc;
        private DmTx4kz202C _tx;

        private Occupancy _occ;
        private Display _display;
        private UI _ui;

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
                _ui = new UI(this);
                _ui.Add(new Tsw1060(0x03, this));
                _ui.RegisterAll();

                _occ = new Occupancy(0x04, this);
                _occ.RoomOccupied += (sender, args) => _ui.SetSource((ushort)SourceIds.RoomPC);
                _occ.RoomVacant += (sender, args) => _ui.SetSource((ushort)SourceIds.None);

                _rmc = new DmRmc4kz100C(0x14, this);

                if (_rmc.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("Unable to register DM-RMC-4KZ-100-C: {0}", _rmc.RegistrationFailureReason);
                }

                _display = new Display(_rmc.ComPorts[1]);

                _tx = new DmTx4kz202C(0x15, this);
                _tx.HdmiInputs[1].InputStreamChange += laptop_StreamChange;
                _tx.HdmiInputs[2].InputStreamChange += laptop_StreamChange;
                
                if (_tx.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("Unable to register DM-TX-4KZ-202-C: {0}", _tx.RegistrationFailureReason);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        public void SystemOn()
        {
            _display.TurnOn();
        }

        public void SystemOff()
        {
            _display.TurnOff();
        }

        private void laptop_StreamChange(EndpointInputStream stream, EndpointInputStreamEventArgs args)
        {
            if (args.EventId == EndpointInputStreamEventIds.SyncDetectedFeedbackEventId)
            {
                var inputStream = stream as EndpointHdmiInput;

                if (inputStream.SyncDetectedFeedback.BoolValue)
                {
                    _ui.SetSource((ushort)SourceIds.RoomPC);   // make sure Room PC is on display
                }
            }
        }
    }
}