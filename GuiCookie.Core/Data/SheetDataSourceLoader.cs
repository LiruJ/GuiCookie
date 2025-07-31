using GuiCookie.Core.Helpers;

namespace GuiCookie.Core.Data
{
    public class SheetDataSourceLoader
    {
        #region Fields
        private static readonly Queue<List<SheetNodePathPair>> nodesBySheetPool = [];

        private readonly Dictionary<string, Func<string?, Stream?, SheetDataSource>> registeredLoaders = [];
        #endregion

        #region Properties
        public IEnumerable<string> RegisteredFileExtensions => registeredLoaders.Keys.AsEnumerable();
        #endregion

        #region Register Functions
        public bool HasRegisteredLoader(string fileExtension)
        {
            fileExtension = PathHelpers.NormaliseExtension(fileExtension);
            return registeredLoaders.ContainsKey(fileExtension);
        }

        public void RegisterLoader(string fileExtension, Func<string?, Stream?, SheetDataSource> loader)
        {
            fileExtension = PathHelpers.NormaliseExtension(fileExtension);

            if (!registeredLoaders.TryAdd(fileExtension, loader))
                throw new ArgumentException($"File extension \"{fileExtension}\" is already registered to a loader!");
        }
        #endregion

        #region Load Functions
        public List<SheetDataSource> TryLoad(IEnumerable<string> filePaths, out List<string> failedPaths)
        {
            List<SheetDataSource> loadedFiles = [];
            failedPaths = [];

            foreach (string filePath in filePaths)
                if (TryLoad(filePath, out SheetDataSource? sheetData))
                    loadedFiles.Add(sheetData!);
                else
                    failedPaths.Add(filePath);

            return loadedFiles;
        }

        public bool TryLoad(string filePath, out SheetDataSource? sheetData)
        {
            string fileExtension = Path.GetExtension(filePath);
            if (!registeredLoaders.TryGetValue(fileExtension, out Func<string?, Stream?, SheetDataSource>? loader))
                throw new ArgumentException($"File extension \"{fileExtension}\" has no registered loader!");

            try
            {
                sheetData = loader(filePath, null);
                return true;
            }
            catch (Exception)
            {
                sheetData = null;
                return false;
            }
        }

        public bool TryLoad(Stream stream, string fileExtension, out SheetDataSource? sheetData)
        {
            fileExtension = PathHelpers.NormaliseExtension(fileExtension);

            if (!registeredLoaders.TryGetValue(fileExtension, out Func<string?, Stream?, SheetDataSource>? loader))
                throw new ArgumentException($"File extension \"{fileExtension}\" has no registered loader!");

            try
            {
                sheetData = loader(null, stream);
                return true;
            }
            catch (Exception)
            {
                sheetData = null;
                return false;
            }
        }

        public IEnumerable<string> ResolveIncluded(IReadOnlySheetDataNode includedSheetsNode, HashSet<string> excludedFilePaths)
        {
            // Try to resolve all filepaths defined in the node.
            List<string> failedPaths = [];
            IEnumerable<string> includedFilePaths = PathHelpers.ResolveFilePaths(includedSheetsNode, RegisteredFileExtensions, failedPaths).Select(PathHelpers.NormaliseFilePath);
            if (failedPaths.Count > 0)
                throw new InvalidDataException($"The following included sheets either had no registered loaders or were missing files: {string.Join('\n', failedPaths)}");

            // Exclude any filepaths that are in the set, and add those that aren't into the set.
            includedFilePaths = includedFilePaths.Where(x => !excludedFilePaths.Contains(x));
            foreach (string filePath in includedFilePaths)
                excludedFilePaths.Add(filePath);

            return includedFilePaths;
        }

        /// <summary>
        /// Resolves all sheets included in the given <paramref name="includedSheetsNode"/>, loads them, then recursively resolves and loads their included sheets, until no new sheets are found.
        /// </summary>
        /// <param name="includedSheetsNode"> The node whose child nodes contain the included sheet paths. </param>
        /// <param name="includeNodeName"> The name of the include node within any loaded sheets, to recursively load their children. </param>
        /// <param name="loadedSheets"> The list which is filled with the loaded sheets. </param>
        /// <param name="loadedFilePaths"> The collection of resolved file paths, this is filled as files are loaded. </param>
        /// <exception cref="InvalidDataException"> When a sheet fails to load. </exception>
        public void ResolveAndLoadIncluded(IReadOnlySheetDataNode includedSheetsNode, string includeNodeName, ref List<SheetDataSource> loadedSheets, HashSet<string>? loadedFilePaths = null)
        {
            // Resolve and load the included sheets within the given node, keeping track of files that have already been loaded to ensure no circular dependencies.
            loadedFilePaths ??= [];
            IEnumerable<string> includedFilePaths = ResolveIncluded(includedSheetsNode, loadedFilePaths);

            // Try to load all of the resolved included files.
            IEnumerable<SheetDataSource> includedSheetSources = TryLoad(includedFilePaths, out List<string> failedSources);
            if (failedSources.Count > 0)
                throw new InvalidDataException($"The following included sheets failed to load: {string.Join('\n', failedSources)}");
            loadedSheets.AddRange(includedSheetSources);

            // Recursively load the included sheets from the loaded sheets. Pass through the already loaded filepaths, so it stops after loading everything.
            foreach (SheetDataSource includedSheet in includedSheetSources)
            {
                IReadOnlySheetDataNode? includeNode = includedSheet.GetChildWithName(includeNodeName);
                if (includeNode == null)
                    continue;
                ResolveAndLoadIncluded(includeNode, includeNodeName, ref loadedSheets, loadedFilePaths);
            }
        }

        public void ResolveAndLoadIncluded(IEnumerable<IReadOnlySheetDataSource> sheetSources, string includeNodeName, ref List<SheetDataSource> loadedSheets, ref HashSet<string> loadedFilePaths)
        {
            foreach (IReadOnlySheetDataSource? sheetSource in sheetSources)
            {
                IReadOnlySheetDataNode? includeNode = sheetSource.GetChildWithName(includeNodeName);
                if (includeNode != null)
                    ResolveAndLoadIncluded(includeNode, includeNodeName, ref loadedSheets, loadedFilePaths);
            }
        }
        #endregion

        #region Sheet Functions
        public static List<SheetNodePathPair> RentNodesBySheetPath(IEnumerable<IReadOnlySheetDataSource> sheetSources, string nodeName)
        {
            if (!nodesBySheetPool.TryDequeue(out List<SheetNodePathPair>? nodesBySheetPath))
                nodesBySheetPath = new(64);

            foreach (IReadOnlySheetDataSource sheetSource in sheetSources)
            {
                string sheetNormalisedPath = PathHelpers.NormaliseFilePath(sheetSource.FilePath ?? throw new ArgumentException("One of the given sheets has no file path!"));
                IReadOnlySheetDataNode? targetNode = sheetSource.GetChildWithName(nodeName);
                if (targetNode == null)
                    continue;
                foreach (IReadOnlySheetDataNode node in targetNode.ChildNodes)
                    nodesBySheetPath.Add(new(node, sheetNormalisedPath));
            }

            return nodesBySheetPath;
        }

        public static void ReturnNodesBySheetPath(List<SheetNodePathPair> nodesBySheetPath)
        {
            if (nodesBySheetPool.Contains(nodesBySheetPath))
                throw new ArgumentException("Cannot return same list twice!", nameof(nodesBySheetPath));
            nodesBySheetPool.Enqueue(nodesBySheetPath);
        }
        #endregion
    }
}
