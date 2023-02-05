using DataWebApi.Models;
using DataWebApi.ResultModels;

namespace DataWebApi.Interfaces
{
    public interface IFilenameRepository
    {
        public Filename? GetFilenameByName(string name);
        public Task<OperationResult> CreateFilename(Filename fileName);
        public Task<OperationResult> Save();
    }
}
