namespace affolterNET.TextExtractor.Storage.Models;

public class StorageAccountKey
{
    private readonly string _content;

    public StorageAccountKey(string connectionString)
    {
        _content = connectionString;
    }

    public static implicit operator string(StorageAccountKey connectionString)
    {
        return connectionString._content;
    }

    public override string ToString()
    {
        return _content;
    }
}