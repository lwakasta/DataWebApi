using DataWebApi.Dto;
using DataWebApi.Models;
using DataWebApi.ResultModels;

namespace DataWebApi.Interfaces
{
    public interface IValueRepository
    {
        public Task<OperationResult> CreateValues(List<Value> values);
        public List<Value> GetValuesByFileId(int fileId);
        public IEnumerable<ValueDto>? GetValuesByFilename(string fileName);
        public Task<OperationResult> DeleteValue(Value value);
        public Task<OperationResult> Save();
    }
}
