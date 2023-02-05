using CsvHelper.Configuration;
using System.Globalization;

namespace DataWebApi.Wrappers
{
    public interface IReaderWrapper
    {
        StreamReader StreamReader(Stream stream);
        CsvConfiguration CsvConfiguration(CultureInfo ci);
        ICsvReaderWrapper CsvReader(StreamReader reader, CsvConfiguration cc);        
    }    
}
