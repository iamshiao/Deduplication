namespace Deduplication.Model.DTO
{
    public class ProgressInfo
    {
        public long Total { get; set; }

        public long Processed { get; set; }

        public string Message { get; set; }

        public ProgressInfo()
        {
            Total = 1;
            Processed = 0;
            Message = "";
        }

        public ProgressInfo(long total, long processed, string message)
        {
            Total = total;
            Processed = processed;
            Message = message;
        }
    }
}
