using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using DataWebApi.Data;
using DataWebApi.Interfaces;
using DataWebApi.Repositories;
using DataWebApi.Services;
using DataWebApi.Services.Implementations;
using DataWebApi.Wrappers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x => 
                        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddScoped<IValueRepository, ValueRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<IFilenameRepository, FilenameRepository>();
builder.Services.AddScoped<IFileImporterService, CsvImporterService>();
builder.Services.AddScoped<IReaderWrapper, ReaderWrapper>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<OperationDbContext>(opt => opt.UseSqlServer(connection));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
