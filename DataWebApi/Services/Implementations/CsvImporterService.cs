using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using DataWebApi.Interfaces;
using DataWebApi.Models;
using DataWebApi.ResultModels;
using DataWebApi.Wrappers;

namespace DataWebApi.Services.Implementations
{
    public class CsvImporterService : IFileImporterService
    {
        private readonly IFilenameRepository _filenameRepository;
        private readonly IValueRepository _valueRepository;
        private readonly IReaderWrapper _reader;

        public CsvImporterService(IFilenameRepository filenameRepository,
            IValueRepository valueRepository,
            IReaderWrapper reader)
        {
            _filenameRepository = filenameRepository;
            _valueRepository = valueRepository;
            _reader = reader;
        }

        public async Task<ImportResult> ImportValues(IFormFile file)
        {
            /*
             * При импорте значений из файла, имя которого уже есть в БД, 
             * должна происходить перезапись уже хранящихся данных из старого файла.
             * При этом возникает одна из следующих ситуаций:
             * 1) Количество строк в новом и старом файлах совпадает.
             * 2) В новом файле строк больше, чем было в старом.
             * 3) В новом файле строк меньше, чем было в старом.
             */

            var values = new List<Value>();
            bool isFileExists = false;

            var fileName = _filenameRepository.GetFilenameByName(file.FileName);
            if (fileName != null)
            {
                isFileExists = true;
                // получаем значения из старого файла, чтобы их перезаписывать
                values = _valueRepository.GetValuesByFileId(fileName.Id);
            }
            else
            {
                fileName = new Filename { Name = file.FileName };
            }

            var config = _reader.CsvConfiguration(CultureInfo.InvariantCulture);

            // индекс для возможности прохода по списку values, если данные перезаписываются
            int index = 0;
            using (var reader = _reader.StreamReader(file.OpenReadStream()))
            using (var csv = _reader.CsvReader(reader, config))
            {
                while (await csv.ReadAsync())
                {
                    /*
                     * Если index меньше, чем количество элементов в списке values, это значит, что файл уже
                     * загружался ранее, и нужно переписывать хранящиеся значения.
                     * 
                     * Если index больше, чем количество элементов в списке values, это означает одно из двух:
                     * 1) происходит импорт значений из нового файла, имени которого нет в БД;
                     * 2) при перезаписи значений оказалось, что в новом файле строк больше, чем было в БД, 
                     *    и далее для вновь прочитанных строк необходимо создавать новые записи
                     */
                    if (index < values.Count)
                    {
                        values[index].Date = DateTime.ParseExact(csv.GetField<string>(0)!, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
                        values[index].Time = csv.GetField<int>(1);
                        values[index].Param = Convert.ToDouble(csv.GetField<string>(2));
                    }
                    else
                    {
                        var record = new Value
                        {
                            FileName = fileName,
                            Date = DateTime.ParseExact(csv.GetField<string>(0)!, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture),
                            Time = csv.GetField<int>(1),
                            Param = Convert.ToDouble(csv.GetField<string>(2))
                        };

                        values.Add(record);
                    }

                    if (!values[index].ValidateValue())
                        return new ImportResult { ErrorMessage = "Некорректный формат данных" };

                    if (values.Count > 10000)
                        return new ImportResult { ErrorMessage = "Количество строк не должно превышать 10000" };

                    index++;
                }
            }

            if (index == 0)
                return new ImportResult { ErrorMessage = "Пустой файл" };

            // если данные перезаписываются, и в новом файле оказалось меньше строк, чем находится в БД - удаляем оставшиеся в БД строки
            for (int i = values.Count - 1; i >= index; i--)
            {
                await _valueRepository.DeleteValue(values[i]);
                values.RemoveAt(i);
            }

            // если файл новый, добавляем его имя в БД
            if (!isFileExists)
            {
                var createFilenameResult = await _filenameRepository.CreateFilename(fileName);
                if (!createFilenameResult.IsSuccessful)
                    return new ImportResult { ErrorMessage = createFilenameResult.ErrorMessage };
            }

            var createResult = await _valueRepository.CreateValues(values);
            if (!createResult.IsSuccessful)
                return new ImportResult { ErrorMessage = createResult.ErrorMessage };

            return new ImportResult { Values = values, IsOverwrite = isFileExists };
        }
    }
}
