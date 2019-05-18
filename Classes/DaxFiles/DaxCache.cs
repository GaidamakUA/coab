using System.Collections.Generic;

namespace Classes.DaxFiles
{
    public static class DaxCache
    {
        private static readonly Dictionary<string, DaxFileCache> FileCache = new Dictionary<string, DaxFileCache>();

        public static byte[] LoadDax(string fileName, int blockId)
        {
            fileName = fileName.ToLower();

            if (!FileCache.TryGetValue(fileName, out var daxFileCache))
            {
                daxFileCache = new DaxFileCache(fileName);
                FileCache.Add(fileName, daxFileCache);
            }

            return daxFileCache.GetData(blockId);
        }
    }
}