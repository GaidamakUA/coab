using System.Drawing;
using System.Drawing.Imaging;

namespace Classes
{
    public enum TextRegion
    {
        NormalBottom,
        Normal2,
        CombatSummary
    }

    public static class Display
    {
        private static readonly byte[,] OrigEgaColors =
        {
            {0, 0, 0}, {0, 0, 173}, {0, 173, 0}, {0, 173, 173}, {173, 0, 0}, {173, 0, 173}, {173, 82, 0},
            {173, 173, 173}, {82, 82, 82}, {82, 82, 255}, {82, 255, 82}, {82, 255, 255}, {255, 82, 82}, {255, 82, 255},
            {255, 255, 82}, {255, 255, 255}
        };

        private static readonly byte[,] EgaColors =
        {
            {0, 0, 0}, {0, 0, 173}, {0, 173, 0}, {0, 173, 173}, {173, 0, 0}, {173, 0, 173}, {173, 82, 0},
            {173, 173, 173}, {82, 82, 82}, {82, 82, 255}, {82, 255, 82}, {82, 255, 255}, {255, 82, 82}, {255, 82, 255},
            {255, 255, 82}, {255, 255, 255}
        };

        private static readonly int[,] Ram;
        private static byte[] _videoRam;
        private static byte[] _videoRamBkUp;
        private static readonly int VideoRamSize;
        private static readonly int ScanLineWidth;
        private static readonly int OutputWidth;
        private static readonly int OutputHeight;

        public static readonly Bitmap Bitmap;
        private static readonly Rectangle Rect = new Rectangle(0, 0, 320, 200);

        public delegate void VoidDelegate();

        private static VoidDelegate _updateCallback;

        public static VoidDelegate UpdateCallback
        {
            set => _updateCallback = value;
        }

        static Display()
        {
            OutputHeight = 200;
            OutputWidth = 320;

            Ram = new int[OutputHeight, OutputWidth];
            ScanLineWidth = OutputWidth * 3;
            VideoRamSize = ScanLineWidth * OutputHeight;
            _videoRam = new byte[VideoRamSize];

            Bitmap = new Bitmap(OutputWidth, OutputHeight, PixelFormat.Format24bppRgb);
        }

        private static readonly int[] MonoBitMask = {0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01};

        public static void DisplayMono8x8(int xCol, int yCol, byte[] monoData8x8, int bgColor, int fgColor)
        {
            var pX = xCol * 8;

            for (var yStep = 0; yStep < 8; yStep++)
            {
                var pY = (yCol * 8) + yStep;
                int value = gbl.monoCharData[yStep];

                for (int i = 0; i < 8; i++)
                {
                    Ram[pY, pX + i] = (value & MonoBitMask[i]) != 0 ? fgColor : bgColor;
                    SetVidPixel(pX + i, pY, Ram[pY, pX + i]);
                }
            }
        }

        public static void SetEgaPalette(int index, int colour)
        {
            EgaColors[index, 0] = OrigEgaColors[colour, 0];
            EgaColors[index, 1] = OrigEgaColors[colour, 1];
            EgaColors[index, 2] = OrigEgaColors[colour, 2];

            for (var y = 0; y < OutputHeight; y++)
            {
                var vy = y * ScanLineWidth;
                for (var x = 0; x < OutputWidth; x++)
                {
                    var vx = x * 3;
                    var egaColor = Ram[y, x];

                    _videoRam[vy + vx + 0] = EgaColors[egaColor, 2];
                    _videoRam[vy + vx + 1] = EgaColors[egaColor, 1];
                    _videoRam[vy + vx + 2] = EgaColors[egaColor, 0];
                }
            }

            Display.Update();
        }

        private static void SetVidPixel(int x, int y, int egaColor)
        {
            _videoRam[(y * ScanLineWidth) + (x * 3) + 0] = EgaColors[egaColor, 2];
            _videoRam[(y * ScanLineWidth) + (x * 3) + 1] = EgaColors[egaColor, 1];
            _videoRam[(y * ScanLineWidth) + (x * 3) + 2] = EgaColors[egaColor, 0];
        }

        static int noUpdateCount;

        public static void UpdateStop()
        {
            noUpdateCount++;
        }

        public static void UpdateStart()
        {
            noUpdateCount--;
            Update();
        }

        public static void Update()
        {
            if (noUpdateCount != 0) return;

            RawCopy(_videoRam, VideoRamSize);

            _updateCallback?.Invoke();
        }

        public static void ForceUpdate()
        {
            RawCopy(_videoRam, VideoRamSize);

            _updateCallback?.Invoke();
        }

        public static void SaveVidRam()
        {
            _videoRamBkUp = (byte[]) _videoRam.Clone();
        }

        public static void RestoreVidRam()
        {
            _videoRam = _videoRamBkUp;
        }

        public static byte GetPixel(int x, int y)
        {
            return (byte) Ram[y, x];
        }

        public static void SetPixel3(int x, int y, int value)
        {
            if (value < 16)
            {
                Ram[y, x] = value;

                SetVidPixel(x, y, Ram[y, x]);
            }

            if (value > 16)
            {
            }
        }


        private static void RawCopy(byte[] videoRam, int videoRamSize)
        {
            var bmpData = Bitmap.LockBits(Rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            var ptr = bmpData.Scan0;

            System.Runtime.InteropServices.Marshal.Copy(videoRam, 0, ptr, videoRamSize);

            Bitmap.UnlockBits(bmpData);
        }
    }
}