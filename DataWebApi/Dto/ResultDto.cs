namespace DataWebApi.Dto
{
    public class ResultDto
    {
        public string FileName { get; set; }
        public long ExecTimeTicks { get; set; }
        public DateTime StartTime { get; set; }
        public int AverageTime { get; set; }
        public double AverageParam { get; set; }
        public double ParamMedian { get; set; }
        public double MaxParam { get; set; }
        public double MinParam { get; set; }
        public int RowsCount { get; set; }
    }
}
