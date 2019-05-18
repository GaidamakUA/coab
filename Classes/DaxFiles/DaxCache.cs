using System.Collections.Generic;

namespace Classes.DaxFiles
{
    public class DaxCache
    {
        private static readonly Dictionary<string, DaxFileCache> FileCache = new Dictionary<string, DaxFileCache>();

        public static byte[] LoadDax(string fileName, int blockId)
        {
            DaxFileCache dfc;

            fileName = fileName.ToLower();

            if (!FileCache.TryGetValue(fileName, out dfc))
            {
                dfc = new DaxFileCache(fileName);
                FileCache.Add(fileName, dfc);
            }

            return dfc.GetData(blockId);
        }
    }
}