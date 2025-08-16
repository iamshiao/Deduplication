using System;

namespace Deduplication.Model.DTO
{
    public class ProgressInfo
    {
        public long Total { get; set; }

        public long Processed { get; set; }

        public string Message { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public string FormattedElapsedTime => $"{(int)ElapsedTime.TotalHours:D2}:{ElapsedTime.Minutes:D2}:{ElapsedTime.Seconds:D2}";

        public ProgressInfo()
        {
            Total = 1;
            Processed = 0;
            Message = "";
            StartTime = DateTime.Now;
            ElapsedTime = TimeSpan.Zero;
        }

        public ProgressInfo(long total, long processed, string message)
        {
            Total = total;
            Processed = processed;
            Message = message;
            StartTime = DateTime.Now;
            ElapsedTime = TimeSpan.Zero;
        }

        public void UpdateElapsedTime()
        {
            ElapsedTime = DateTime.Now - StartTime;
        }
    }
}
