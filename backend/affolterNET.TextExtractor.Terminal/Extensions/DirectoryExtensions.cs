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

        var folders = di.GetFolders(folderNamePredicate);
        foreach (var fo in folders)
        {
            foreach (var fi in fo.GetFiles(pattern))
            {
                yield return fi;
            }
        }
    }

    public static List<DirectoryInfo> GetFolders(this DirectoryInfo root, Func<List<DirectoryInfo>, List<DirectoryInfo>>? folderNamePredicate = null)
    {
        
        var list = new List<DirectoryInfo> { root };
        var selected = root.GetDirectories().ToList();
        if (folderNamePredicate != null)
        {
            selected = folderNamePredicate(selected);
        }
        list.AddRange(selected);
        return list;
    }
}