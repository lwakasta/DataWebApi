using CsvHelper.Configuration;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataWebApi.Interfaces;
using DataWebApi.Models;
using DataWebApi.ResultModels;
using DataWebApi.Services;
using DataWebApi.Services.Implementations;
using DataWebApi.Wrappers;

namespace DataWebApi.Tests.Services
{
    public class CsvImporterServiceTests
    {
        private readonly IFilenameRepository _filenameRepository;
        private readonly IValueRepository _valueRepository;
        private readonly IReaderWrapper _reader;

        public CsvImporterServiceTests()
        {
            _filenameRepository = A.Fake<IFilenameRepository>();
            _valueRepository = A.Fake<IValueRepository>();
            _reader = A.Fake<IReaderWrapper>();
        }

        [Fact]
        public void CsvImporterService_ImportValues_ReturnsSuccess()
        {
            // Arrange
            var file = A.Fake<IFormFile>();
            var filename = new Filename { Id = 1, Name = "file" };
            A.CallTo(() => _filenameRepository.GetFilenameByName(A<string>.Ignored)).Returns(filename);

            string fileContent = "test";
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            MemoryStream ms = new MemoryStream(fileBytes);

            var cultureInfo = CultureInfo.InvariantCulture;
            var config = new CsvConfiguration(cultureInfo);
            var streamReader = new StreamReader(ms);
            var fakeCsvReader = A.Fake<ICsvReaderWrapper>();
            A.CallTo(() => _reader.CsvConfiguration(A<CultureInfo>.Ignored)).Returns(config);
            A.CallTo(() => _reader.StreamReader(A<Stream>.Ignored)).Returns(streamReader);
            A.CallTo(() => _reader.CsvReader(A<StreamReader>.Ignored, A<CsvConfiguration>.Ignored)).Returns(fakeCsvReader);
            A.CallTo(() => fakeCsvReader.ReadAsync()).ReturnsNextFromSequence(true, false);
            A.CallTo(() => fakeCsvReader.GetField<string>(0)).Returns("2020-01-01_01-01-01");
            A.CallTo(() => fakeCsvReader.GetField<int>(1)).Returns(1);
            A.CallTo(() => fakeCsvReader.GetField<string>(2)).Returns("1,2");
            var operationResult = new OperationResult { IsSuccessful = true };
            A.CallTo(() => _valueRepository.CreateValues(A<List<Value>>.Ignored)).Returns(operationResult);
            var service = new CsvImporterService(_filenameRepository, _valueRepository, _reader);

            // Act
            var result = service.ImportValues(file);

            // Assert
            Assert.NotNull(result.Result.Values);
        }

        [Fact]
        public void CsvImporterService_ImportValues_ReturnsOther10000LinesErrorMessage()
        {
            // Arrange
            var file = A.Fake<IFormFile>();
            var filename = new Filename { Id = 1, Name = "file" };
            A.CallTo(() => _filenameRepository.GetFilenameByName(A<string>.Ignored)).Returns(filename);

            string fileContent = "test";
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            MemoryStream ms = new MemoryStream(fileBytes);

            var cultureInfo = CultureInfo.InvariantCulture;
            var config = new CsvConfiguration(cultureInfo);
            var streamReader = new StreamReader(ms);
            var fakeCsvReader = A.Fake<ICsvReaderWrapper>();
            A.CallTo(() => _reader.CsvConfiguration(A<CultureInfo>.Ignored)).Returns(config);
            A.CallTo(() => _reader.StreamReader(A<Stream>.Ignored)).Returns(streamReader);
            A.CallTo(() => _reader.CsvReader(A<StreamReader>.Ignored, A<CsvConfiguration>.Ignored)).Returns(fakeCsvReader);
            A.CallTo(() => fakeCsvReader.ReadAsync()).Returns(true);
            A.CallTo(() => fakeCsvReader.GetField<string>(0)).Returns("2020-01-01_01-01-01");
            A.CallTo(() => fakeCsvReader.GetField<int>(1)).Returns(1);
            A.CallTo(() => fakeCsvReader.GetField<string>(2)).Returns("1,2");
            var service = new CsvImporterService(_filenameRepository, _valueRepository, _reader);

            // Act
            var result = service.ImportValues(file);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Количество строк не должно превышать 10000", result.Result.ErrorMessage);
        }

        [Fact]
        public void CsvImporterService_ImportValues_ReturnsInvalidFormatErrorMessage()
        {
            // Arrange
            var file = A.Fake<IFormFile>();
            A.CallTo(() => _filenameRepository.GetFilenameByName(A<string>.Ignored)).Returns(null);

            string fileContent = "test";
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            MemoryStream ms = new MemoryStream(fileBytes);

            var cultureInfo = CultureInfo.InvariantCulture;
            var config = new CsvConfiguration(cultureInfo);
            var streamReader = new StreamReader(ms);
            var fakeCsvReader = A.Fake<ICsvReaderWrapper>();
            A.CallTo(() => _reader.CsvConfiguration(A<CultureInfo>.Ignored)).Returns(config);
            A.CallTo(() => _reader.StreamReader(A<Stream>.Ignored)).Returns(streamReader);
            A.CallTo(() => _reader.CsvReader(A<StreamReader>.Ignored, A<CsvConfiguration>.Ignored)).Returns(fakeCsvReader);
            A.CallTo(() => fakeCsvReader.ReadAsync()).Returns(true);
            A.CallTo(() => fakeCsvReader.GetField<string>(0)).Returns("1999-01-01_01-01-01");
            A.CallTo(() => fakeCsvReader.GetField<int>(1)).Returns(1);
            A.CallTo(() => fakeCsvReader.GetField<string>(2)).Returns("1,2");
            var service = new CsvImporterService(_filenameRepository, _valueRepository, _reader);

            // Act
            var result = service.ImportValues(file);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Некорректный формат данных", result.Result.ErrorMessage);
        }

        [Fact]
        public void CsvImporterService_ImportValues_ReturnsEmptyFileErrorMessage()
        {
            // Arrange
            var file = A.Fake<IFormFile>();
            A.CallTo(() => _filenameRepository.GetFilenameByName(A<string>.Ignored)).Returns(null);

            string fileContent = "test";
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            MemoryStream ms = new MemoryStream(fileBytes);

            var cultureInfo = CultureInfo.InvariantCulture;
            var config = new CsvConfiguration(cultureInfo);
            var streamReader = new StreamReader(ms);
            var fakeCsvReader = A.Fake<ICsvReaderWrapper>();
            A.CallTo(() => _reader.CsvConfiguration(A<CultureInfo>.Ignored)).Returns(config);
            A.CallTo(() => _reader.StreamReader(A<Stream>.Ignored)).Returns(streamReader);
            A.CallTo(() => _reader.CsvReader(A<StreamReader>.Ignored, A<CsvConfiguration>.Ignored)).Returns(fakeCsvReader);
            A.CallTo(() => fakeCsvReader.ReadAsync()).Returns(false);
            var service = new CsvImporterService(_filenameRepository, _valueRepository, _reader);

            // Act
            var result = service.ImportValues(file);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Пустой файл", result.Result.ErrorMessage);
        }

        [Fact]
        public void CsvImporterService_ImportValues_ReturnsCreateFilenameErrorMessage()
        {
            // Arrange
            var file = A.Fake<IFormFile>();
            A.CallTo(() => _filenameRepository.GetFilenameByName(A<string>.Ignored)).Returns(null);

            string fileContent = "test";
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            MemoryStream ms = new MemoryStream(fileBytes);

            var cultureInfo = CultureInfo.InvariantCulture;
            var config = new CsvConfiguration(cultureInfo);
            var streamReader = new StreamReader(ms);
            var fakeCsvReader = A.Fake<ICsvReaderWrapper>();
            A.CallTo(() => _reader.CsvConfiguration(A<CultureInfo>.Ignored)).Returns(config);
            A.CallTo(() => _reader.StreamReader(A<Stream>.Ignored)).Returns(streamReader);
            A.CallTo(() => _reader.CsvReader(A<StreamReader>.Ignored, A<CsvConfiguration>.Ignored)).Returns(fakeCsvReader);
            A.CallTo(() => fakeCsvReader.ReadAsync()).ReturnsNextFromSequence(true, false);
            A.CallTo(() => fakeCsvReader.GetField<string>(0)).Returns("2020-01-01_01-01-01");
            A.CallTo(() => fakeCsvReader.GetField<int>(1)).Returns(1);
            A.CallTo(() => fakeCsvReader.GetField<string>(2)).Returns("1,2");
            var operationResult = new OperationResult { IsSuccessful = false, ErrorMessage = "Error" };
            A.CallTo(() => _filenameRepository.CreateFilename(A<Filename>.Ignored)).Returns(operationResult);
            var service = new CsvImporterService(_filenameRepository, _valueRepository, _reader);

            // Act
            var result = service.ImportValues(file);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Error", result.Result.ErrorMessage);
        }

        [Fact]
        public void CsvImporterService_ImportValues_ReturnsCreateValuesErrorMessage()
        {
            // Arrange
            var file = A.Fake<IFormFile>();
            var filename = new Filename { Id = 1, Name = "file" };
            A.CallTo(() => _filenameRepository.GetFilenameByName(A<string>.Ignored)).Returns(filename);

            string fileContent = "test";
            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            MemoryStream ms = new MemoryStream(fileBytes);

            var cultureInfo = CultureInfo.InvariantCulture;
            var config = new CsvConfiguration(cultureInfo);
            var streamReader = new StreamReader(ms);
            var fakeCsvReader = A.Fake<ICsvReaderWrapper>();
            A.CallTo(() => _reader.CsvConfiguration(A<CultureInfo>.Ignored)).Returns(config);
            A.CallTo(() => _reader.StreamReader(A<Stream>.Ignored)).Returns(streamReader);
            A.CallTo(() => _reader.CsvReader(A<StreamReader>.Ignored, A<CsvConfiguration>.Ignored)).Returns(fakeCsvReader);
            A.CallTo(() => fakeCsvReader.ReadAsync()).ReturnsNextFromSequence(true, false);
            A.CallTo(() => fakeCsvReader.GetField<string>(0)).Returns("2020-01-01_01-01-01");
            A.CallTo(() => fakeCsvReader.GetField<int>(1)).Returns(1);
            A.CallTo(() => fakeCsvReader.GetField<string>(2)).Returns("1,2");
            var operationResult = new OperationResult { IsSuccessful = false, ErrorMessage = "Error" };
            A.CallTo(() => _valueRepository.CreateValues(A<List<Value>>.Ignored)).Returns(operationResult);
            var service = new CsvImporterService(_filenameRepository, _valueRepository, _reader);

            // Act
            var result = service.ImportValues(file);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Error", result.Result.ErrorMessage);
        }
    }
}
