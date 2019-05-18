namespace Classes.DaxFiles
{
    class DaxHeaderEntry
    {
        internal int Id;
        internal int Offset;
        internal int DataSize; // decodeSize
        internal int CompressedSize; // dataLength
    }
}