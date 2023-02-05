using System.ComponentModel.DataAnnotations;

namespace DataWebApi.Models
{
    public class ResultParameters
    {
        public string? FileName { get; set; } = null;
        public DateTime MinStartTime { get; set; } = DateTime.MinValue;
        public DateTime MaxStartTime { get; set; } = DateTime.MaxValue;

        [Range(0, Double.MaxValue)]
        public double MinAverageParam { get; set; } = 0;

        [Range(0, Double.MaxValue)]
        public double MaxAverageParam { get; set; } = Double.MaxValue;

        [Range(0, Int32.MaxValue)]
        public int MinAverageTime { get; set; } = 0;

        [Range(0, Int32.MaxValue)]
        public int MaxAverageTime { get; set; } = Int32.MaxValue;
        public bool IsValid => MinStartTime <= MaxStartTime &&
            MinAverageParam <= MaxAverageParam &&
            MinAverageTime <= MaxAverageTime;        
    }
}
