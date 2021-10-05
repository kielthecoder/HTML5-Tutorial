using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.UI;

namespace HuddleRoom
{
    public class ControlSystem : CrestronControlSystem
    {
        private BasicTriListWithSmartObject _tp;
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
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
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
                            _src = args.Sig.UShortValue;
                            break;
                    }
                    break;
            }
        }
    }
}