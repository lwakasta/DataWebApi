using DataWebApi.Models;

namespace DataWebApi.ResultModels
{
    public class ImportResult
    {
        public List<Value>? Values { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsOverwrite { get; set; }
    }
}
