using System;

namespace Classes
{
    public class DaxBlock
    {
        private static readonly Random RandomNumber = new Random(unchecked((int) System.DateTime.Now.Ticks));

        public readonly int Height; // 0x0;
        public readonly int Width; // 0x2;
        public int XPos; // 0x4;
        public int YPos; // 0x6;

        public readonly int ItemCount; // 0x8;
        // public byte[] field_9; // 0x9; byte[8] // Seems unused

        /// <summary>0x11 Bytes Per Picture</summary>
        public readonly int Bpp; // 0x11;

        //public byte[] data_ptr; // 0x13;
        public readonly byte[] ImageData; // 0x17;

        public DaxBlock(int itemCount, int width, int height)
        {
            Height = height;
            Width = width;
            Bpp = Height * Width * 8;
            ItemCount = itemCount;
            int ram_size = ItemCount * Bpp;

            ImageData = new byte[ram_size];
        }

        public DaxBlock(byte[] picData, bool shouldMask, int maskColor)
        {
            Height = Sys.ArrayToShort(picData, 0);
            Width = Sys.ArrayToShort(picData, 2);
            Bpp = Height * Width * 8;
            int x_pos = Sys.ArrayToShort(picData, 4);
            int y_pos = Sys.ArrayToShort(picData, 6);
            ItemCount = picData[8];

            var ramSize = ItemCount * Bpp;
            ImageData = new byte[ramSize];

            const int picDataOffset = 17;
            DaxToPicture(maskColor, shouldMask, picDataOffset, picData);
        }

        public void DaxToPicture(int maskColour, bool shouldMask, int blockOffset, byte[] byteData)
        {
            var destOffset = 0;

            for (var loop1Var = 1; loop1Var <= ItemCount; loop1Var++)
            {
                for (var loop2Var = 0; loop2Var < Height; loop2Var++)
                {
                    for (var loop3Var = 0; loop3Var < (Width * 4); loop3Var++)
                    {
                        var c = byteData[blockOffset];

                        SetMaskedColor(destOffset, c >> 4, shouldMask, maskColour);

                        destOffset += 1;

                        SetMaskedColor(destOffset, c & 0b1111, shouldMask, maskColour);

                        destOffset += 1;
                        blockOffset += 1;
                    }
                }
            }
        }

        private void SetMaskedColor(int offset, int color, bool shouldMask, int maskColor)
        {
            if (shouldMask && color == maskColor)
            {
                ImageData[offset] = 16;
            }
            else
            {
                ImageData[offset] = (byte) color;
            }
        }

        public void FlipIconLeftToRight()
        {
            var tData = new byte[ImageData.Length];

            var tWidth = Width * 8;
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < tWidth; x++)
                {
                    var di = (y * tWidth) + x;
                    var si = (y * tWidth) + (tWidth - x) - 1;

                    tData[di] = ImageData[si];
                }
            }

            Array.Copy(tData, ImageData, ImageData.Length);
        }

        public void Recolor(bool useRandom, byte[] newColors, byte[] oldColors)
        {
            for (var colorIdx = 0; colorIdx < 16; colorIdx++)
            {
                if (oldColors[colorIdx] == newColors[colorIdx]) continue;
                var offset = 0;

                for (var posY = 0; posY < Height; posY++)
                {
                    for (var posX = 0; posX < (Width * 8); posX++)
                    {
                        if (ImageData[offset] == oldColors[colorIdx] &&
                            (useRandom == false || ((RandomNumber.Next() % 4) == 0)))
                        {
                            ImageData[offset] = newColors[colorIdx];
                        }

                        offset += 1;
                    }
                }
            }
        }

        public void MergeIcons(DaxBlock srcIcon) /* icon_xx, could be implemented using alpha-blending */
        {
            for (var i = 0; i < srcIcon.Bpp; i++)
            {
                var a = ImageData[i];
                var b = srcIcon.ImageData[i];

                if (a == 16 && b == 16)
                {
                    ImageData[i] = 16;
                }
                else if (a == 16)
                {
                    ImageData[i] = b;
                }
                else if (b == 16)
                {
                    ImageData[i] = a;
                }
                else
                {
                    //TODO - not sure about this... more likely there should be a presedant, not just blending on the color code..
                    ImageData[i] = (byte) (a | b);
                }
            }
        }
    }
}