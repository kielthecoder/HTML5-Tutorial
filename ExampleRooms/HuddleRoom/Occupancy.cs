using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.GeneralIO;

namespace HuddleRoom
{
    class Occupancy
    {
        private CenOdtCPoe _sensor;

        public event EventHandler RoomOccupied;
        public event EventHandler RoomVacant;

        public Occupancy(uint ipId, CrestronControlSystem cs)
        {
            if (cs.SupportsEthernet)
            {
                _sensor = new CenOdtCPoe(ipId, cs);
                _sensor.CenOccupancySensorChange += _sensor_Change;

                if (_sensor.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("Occupancy: Error registering CEN-ODT-C-POE ({0})", _sensor.RegistrationFailureReason);
                }
            }
            else
            {
                ErrorLog.Error("Occupancy: CEN-ODT-C-POE requires Ethernet support");
            }
        }

        private void _sensor_Change(object sender, GenericEventArgs args)
        {
            switch (args.EventId)
            {
                case GlsOccupancySensorBase.RoomOccupiedFeedbackEventId:
                    if (_sensor.OccupancyDetectedFeedback.BoolValue)
                    {
                        if (RoomOccupied != null)
                        {
                            RoomOccupied(this, new EventArgs());
                        }
                    }
                    break;
                case GlsOccupancySensorBase.RoomVacantFeedbackEventId:
                    if (_sensor.VacancyDetectedFeedback.BoolValue)
                    {
                        if (RoomVacant != null)
                        {
                            RoomVacant(this, new EventArgs());
                        }
                    }
                    break;
            }
        }
    }
}
