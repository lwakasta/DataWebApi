using CsvHelper.Configuration;
using CsvHelper;

namespace DataWebApi.Wrappers
{
    public class CsvReaderWrapper : ICsvReaderWrapper, IDisposable
    {
        private readonly CsvReader _csvReader;
        public CsvReaderWrapper(StreamReader reader, CsvConfiguration cc)
        {
            _csvReader = new CsvReader(reader, cc);
        }

        public Task<bool> ReadAsync()
        {
            return _csvReader.ReadAsync();
        }

        public T GetField<T>(int index)
        {
            return _csvReader.GetField<T>(index);
        }

        public void Dispose()
        {
            _csvReader.Dispose();
        }
    }
}
