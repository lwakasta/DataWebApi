using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace DataWebApi.Wrappers
{
    public class ReaderWrapper : IReaderWrapper
    {
        public CsvConfiguration CsvConfiguration(CultureInfo ci)
        {
            return new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ";"
            };
        }

        public ICsvReaderWrapper CsvReader(StreamReader reader, CsvConfiguration cc)
        {
            return new CsvReaderWrapper(reader, cc);
        }

        public StreamReader StreamReader(Stream stream)
        {
            return new StreamReader(stream);
        }
    }    
}
