namespace SettlementServiceWebAPI.Models
{
    public class TimeSlot
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
