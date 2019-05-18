using System.Collections.Generic;

namespace Classes.DaxFiles
{
    class DaxFileCache
    {
        private readonly Dictionary<int, byte[]> _entries;

        internal DaxFileCache(string filename)
        {
            _entries = new Dictionary<int, byte[]>();

            LoadFile(filename);
        }

        private void LoadFile(string filename)
        {
            System.IO.BinaryReader fileReader;

            System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open,
                System.IO.FileAccess.Read, System.IO.FileShare.Read);

            fileReader = new System.IO.BinaryReader(fileStream);

            int dataOffset = fileReader.ReadInt16();

            List<DaxHeaderEntry> headers = new List<DaxHeaderEntry>();

            const int headerEntrySize = 9;

            for (int i = 0; i < (dataOffset / headerEntrySize); i++)
            {
                DaxHeaderEntry header = new DaxHeaderEntry();
                header.Id = fileReader.ReadByte();
                header.Offset = fileReader.ReadInt32();
                header.DataSize = fileReader.ReadInt16();
                header.CompressedSize = fileReader.ReadUInt16();

                headers.Add(header);
            }

            foreach (DaxHeaderEntry header in headers)
            {
                byte[] compressed = fileReader.ReadBytes(header.CompressedSize);

                byte[] data = Extract(header.DataSize, compressed);

                _entries.Add(header.Id, data);
                // next entry should start with header.Offset but it also starts with cursor position
                // check would be nice but isn't necessary
            }

            fileReader.Close();
        }

        private byte[] Extract(int outputSize, byte[] compressed)
        {
            var output = new byte[outputSize];

            var inputIndex = 0;
            var outputIndex = 0;

            while (inputIndex < compressed.Length)
            {
                var runLength = (sbyte) compressed[inputIndex];

                if (runLength >= 0)
                {
                    for (int i = 0; i <= runLength; i++)
                    {
                        output[outputIndex + i] = compressed[inputIndex + i + 1];
                    }

                    inputIndex += runLength + 2;
                    outputIndex += runLength + 1;
                }
                else
                {
                    runLength = (sbyte) (-runLength);

                    for (int i = 0; i < runLength; i++)
                    {
                        output[outputIndex + i] = compressed[inputIndex + 1];
                    }

                    inputIndex += 2;
                    outputIndex += runLength;
                }
            }

            return output;
        }

        internal byte[] GetData(int blockId)
        {
            byte[] orig;
            if (_entries.TryGetValue(blockId, out orig) == false)
            {
                return null;
            }

            return (byte[]) orig.Clone();
        }
    }
}