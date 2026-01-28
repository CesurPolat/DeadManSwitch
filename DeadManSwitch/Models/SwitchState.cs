using System;

namespace DeadManSwitch.Models
{
    public class SwitchState
    {
        public DateTime LastPing { get; set; } = DateTime.MinValue;
        public bool IsActive { get; set; } = true;
        public string Status => (DateTime.UtcNow - LastPing).TotalHours > 24 ? "ALARM" : "OK";
        public DateTime NextCheckExpected => LastPing.AddHours(24);
    }

    public interface ISwitchStore
    {
        SwitchState GetState();
        void UpdatePing();
    }

    public class MemorySwitchStore : ISwitchStore
    {
        private SwitchState _state = new SwitchState();

        public SwitchState GetState()
        {
            return _state;
        }

        public void UpdatePing()
        {
            _state.LastPing = DateTime.UtcNow;
            _state.IsActive = true;
        }
    }
}
