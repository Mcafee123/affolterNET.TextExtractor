namespace affolterNET.TextExtractor.Terminal.Extensions;

public static class DirectoryExtensions
{
    public static IEnumerable<FileInfo> GetFilesInAllDirectories(this string rootPath, string pattern = "*.pdf",
        Func<List<DirectoryInfo>, List<DirectoryInfo>>? folderNamePredicate = null)
    {
        if (string.IsNullOrEmpty(rootPath))
        {
            rootPath = Path.Combine(Environment.CurrentDirectory, "..", "SocialContractData", "Fedlex");
        }

        if (!Path.IsPathRooted(rootPath))
        {
            rootPath = Path.Combine(Environment.CurrentDirectory, "..", "SocialContractData", "Fedlex",
                rootPath);
        }

        var di = new DirectoryInfo(rootPath);
        if (!di.Exists)
        {
            throw new DirectoryNotFoundException($"directory \"{di.FullName}\" does not exist");
        }

        return di.GetFilesInAllDirectories(folderNamePredicate, pattern);
    }
    
    public static IEnumerable<FileInfo> GetFilesInAllDirectories(this DirectoryInfo folder, Func<List<DirectoryInfo>, List<DirectoryInfo>>? folderNamePredicate = null, 
        string pattern = "*.pdf")
    {
        foreach (var fi in folder.GetFiles(pattern))
        {
            yield return fi;
        }   

        var selected = folder.GetDirectories().ToList();
        if (folderNamePredicate != null)
        {
            selected = folderNamePredicate(selected);
        }
        foreach (var di in selected)
        {
            foreach (var fi in di.GetFilesInAllDirectories(folderNamePredicate, pattern))
            {
                yield return fi;
            }
        }
    }
}