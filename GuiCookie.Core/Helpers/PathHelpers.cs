using GuiCookie.Core.Data;

namespace GuiCookie.Core.Helpers
{
    public static class PathHelpers
    {
        // At some point supporting wildcards in the middle of paths would be nice. Directory.EnumerateFiles has an override, maybe look into this

        #region Constants
        private const char wildcardCharacter = '*';

        public const string RootFolderAttributeName = "RootFolder";
        public const string SourceAttributeName = "Source";
        #endregion

        public static IEnumerable<string> ResolveFilePaths(IReadOnlySheetDataNode node, IEnumerable<string> possibleExtensions, IList<string>? failedPaths)
        {
            string rootPath = node.Attributes.GetAttributeOrDefault(RootFolderAttributeName, string.Empty)!;
            IEnumerable<string?> sourcePaths = node.ChildNodes.Select(x => x.Attributes.GetAttributeOrDefault(SourceAttributeName, (string?)null));

            if (failedPaths != null && sourcePaths.Any(string.IsNullOrWhiteSpace))
                 throw new InvalidOperationException($"Some nodes had invalid or missing \"{SourceAttributeName}\" attributes!");

            return ResolveFilePaths(sourcePaths.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!), possibleExtensions, rootPath, failedPaths);
        }

        public static IEnumerable<string> ResolveFilePaths(IEnumerable<string> paths, IEnumerable<string> possibleExtensions, string rootPath, IList<string>? failedPaths)
        {
            List<string> resolvedPaths = [];
            foreach (string path in paths)
            {
                string fullPath = Path.Combine(rootPath, path);

                // Path's filename is a wildcard '*'.
                string filename = Path.GetFileNameWithoutExtension(fullPath);
                if (filename.Length == 1 && filename[0] == wildcardCharacter)
                {
                    string wildcardPath = Path.GetDirectoryName(fullPath) ?? rootPath;
                    string? fileExtension = Path.GetExtension(fullPath);
                    foreach (string wildcardFilePath in Directory.EnumerateFiles(wildcardPath))
                        ResolveFilePath(wildcardFilePath, string.IsNullOrEmpty(fileExtension) ? possibleExtensions : [fileExtension], ref resolvedPaths, failedPaths);
                }
                else
                    ResolveFilePath(fullPath, possibleExtensions, ref resolvedPaths, failedPaths);
            }
            return resolvedPaths;
        }

        public static void ResolveFilePath(string fullPath, IEnumerable<string> possibleExtensions, ref List<string> resolvedPaths, IList<string>? failedPaths)
        {
            string? fileExtension = Path.GetExtension(fullPath);
            if (!string.IsNullOrWhiteSpace(fileExtension))
            {
                // The path has an extension and it is in the list.
                if (possibleExtensions.Any(x => !string.IsNullOrWhiteSpace(x) && (x[0] == '.' ? x == fileExtension : x == fileExtension[1..])))
                {
                    if (File.Exists(fullPath))
                        resolvedPaths.Add(fullPath);
                    else
                        failedPaths?.Add(fullPath);
                }
                // The path has an extension, but it's not in the list of possible ones.
                else
                    failedPaths?.Add(fullPath);
            }
            // Path is not a wildcard and has no extension.
            else
            {
                // Iterate over all possible file extensions, add any files that exist.
                int previousCount = resolvedPaths.Count;
                foreach (string possibleExtension in possibleExtensions)
                {
                    string pathWithPossibleExtension = Path.ChangeExtension(fullPath, possibleExtension);
                    if (File.Exists(pathWithPossibleExtension))
                        resolvedPaths.Add(pathWithPossibleExtension);
                }
                if (previousCount == resolvedPaths.Count)
                    failedPaths?.Add(fullPath);
            }
        }
    }
}
