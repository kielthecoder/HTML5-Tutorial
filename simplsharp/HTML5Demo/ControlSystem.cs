using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.UI;
using General;

namespace HTML5Demo
{
    public class ControlSystem : CrestronControlSystem
    {
        private Contract _contract;
        private BasicTriListWithSmartObject _tp;

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
                _contract = new Contract();

                _contract.Room.Power_On += Room_Power_On;
                _contract.Room.Power_Off += Room_Power_Off;
                
                _tp = new Ts770(0x03, this);
                _tp.OnlineStatusChange += tp_OnlineStatusChange;

                _contract.AddDevice(_tp);

                if (_tp.Register() != eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    ErrorLog.Error("Unable to register TS-770: {0}", _tp.RegistrationFailureReason);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        private void tp_OnlineStatusChange(GenericBase dev, OnlineOfflineEventArgs args)
        {
            _contract.Room.Name_Fb((StringInputSig sig, IRoom room) => sig.StringValue = "SIMPL# Pro Lab");
            _contract.Room.Power_On_Fb((BoolInputSig sig, IRoom room) => sig.BoolValue = true);
            _contract.Room.Power_On_Fb((BoolInputSig sig, IRoom room) => sig.BoolValue = false);
            _contract.Room.Power_Off_Fb((BoolInputSig sig, IRoom room) => sig.BoolValue = true);
        }

        private void Room_Power_On(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                _contract.Room.Power_Off_Fb((BoolInputSig sig, IRoom room) => sig.BoolValue = false);
                _contract.Room.Power_On_Fb((BoolInputSig sig, IRoom room) => sig.BoolValue = true);
            }
        }

        private void Room_Power_Off(object sender, UIEventArgs e)
        {
            if (e.SigArgs.Sig.BoolValue)
            {
                _contract.Room.Power_On_Fb((BoolInputSig sig, IRoom room) => sig.BoolValue = false);
                _contract.Room.Power_Off_Fb((BoolInputSig sig, IRoom room) => sig.BoolValue = true);
            }
        }
    }
}