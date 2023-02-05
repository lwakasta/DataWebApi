namespace DataWebApi.Wrappers
{
    public interface ICsvReaderWrapper : IDisposable
    {
        Task<bool> ReadAsync();
        T GetField<T>(int index);
    }
}
