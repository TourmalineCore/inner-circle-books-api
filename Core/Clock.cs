using NodaTime;

namespace Core
{
    public class Clock : IClock
    {
        public Instant GetCurrentInstant()
        {
            return NodaConstants.BclEpoch.PlusTicks(DateTime.UtcNow.Ticks);
        }
    }
}