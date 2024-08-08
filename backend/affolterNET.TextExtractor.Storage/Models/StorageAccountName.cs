namespace affolterNET.TextExtractor.Storage.Models;

public class StorageAccountName
{
    private readonly string _content;

    public StorageAccountName(string connectionString)
    {
        _content = connectionString;
    }

    public static implicit operator string(StorageAccountName connectionString)
    {
        return connectionString._content;
    }

    public override string ToString()
    {
        return _content;
    }
}