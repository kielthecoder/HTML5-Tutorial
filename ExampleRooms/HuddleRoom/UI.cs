using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;

namespace HuddleRoom
{
    class UI
    {
        private List<BasicTriListWithSmartObject> _panels;
        private ControlSystem _cs;

        private ushort _src;

        public UI(ControlSystem cs)
        {
            _panels = new List<BasicTriListWithSmartObject>();
            _cs = cs;
            _src = 0;
        }

        public void Add(BasicTriListWithSmartObject tp)
        {
            tp.OnlineStatusChange += _tp_OnlineStatusChange;
            tp.SigChange += _tp_SigChange;

            _panels.Add(tp);
        }

        public void RegisterAll()
        {
            foreach (var tp in _panels)
            {
                if (tp.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("UI: Unable to register device ({0})", tp.RegistrationFailureReason);
                }
            }
        }

        public void SetUShort(ushort sig, ushort val)
        {
            foreach (var tp in _panels)
            {
                tp.UShortInput[sig].UShortValue = val;
            }
        }

        public void SetSource(ushort newSource)
        {
            _src = newSource;

            if (_src == (ushort)SourceIds.None)
            {
                _cs.SystemOff();
            }
            else
            {
                _cs.SystemOn();
            }

            SetUShort(1, _src);
        }

        private void _tp_OnlineStatusChange(GenericBase dev, OnlineOfflineEventArgs args)
        {
            if (args.DeviceOnLine)
            {
                var tp = dev as BasicTriListWithSmartObject;

                tp.StringInput[1].StringValue = "Huddle Room";  // Room Name
                tp.StringInput[2].StringValue = "x1234";        // Help Number

                tp.UShortInput[1].UShortValue = _src;           // Currently selected source
            }
        }

        private void _tp_SigChange(BasicTriList dev, SigEventArgs args)
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
    }
}
