using DataWebApi.Dto;
using DataWebApi.Models;
using DataWebApi.ResultModels;

namespace DataWebApi.Interfaces
{
    public interface IResultRepository
    {
        public Task<OperationResult> CreateResult(List<Value> values, Filename fileName, bool isOverwrite);
        public IEnumerable<ResultDto> GetResults(ResultParameters resultParams);
        public Task<OperationResult> UpdateResult(Result result);
        public Task<OperationResult> Save();
    }
}
