using GuiCookie.Core.Helpers;

namespace GuiCookie.Core.Data
{
    public class SheetDataSourceLoader
    {
        #region Fields
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
        #endregion
    }
}
