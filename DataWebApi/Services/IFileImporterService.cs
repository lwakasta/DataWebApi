using DataWebApi.ResultModels;

namespace DataWebApi.Services
{
    public interface IFileImporterService
    {
        public Task<ImportResult> ImportValues(IFormFile file);
    }
}
