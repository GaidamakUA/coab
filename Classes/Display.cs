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
        private static readonly Color[] OrigEgaColors =
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(0, 0, 173),
            Color.FromArgb(0, 173, 0),
            Color.FromArgb(0, 173, 173),
            Color.FromArgb(173, 0, 0),
            Color.FromArgb(173, 0, 173),
            Color.FromArgb(173, 82, 0),
            Color.FromArgb(173, 173, 173),
            Color.FromArgb(82, 82, 82),
            Color.FromArgb(82, 82, 255),
            Color.FromArgb(82, 255, 82),
            Color.FromArgb(82, 255, 255),
            Color.FromArgb(255, 82, 82),
            Color.FromArgb(255, 82, 255),
            Color.FromArgb(255, 255, 82),
            Color.FromArgb(255, 255, 255)
        };

        private static readonly Color[] EgaColours = (Color[]) OrigEgaColors.Clone();

        private static readonly int[,] Ram;
        private static Bitmap _videoRam = new Bitmap(OutputWidth, OutputHeight, PixelFormat.Format24bppRgb);
        private static Bitmap _videoRamBkUp;
        private const int OutputWidth = 320;
        private const int OutputHeight = 200;

        public static Bitmap Bitmap;

        public delegate void VoidDelegate();

        private static VoidDelegate _updateCallback;

        public static VoidDelegate UpdateCallback
        {
            set => _updateCallback = value;
        }

        static Display()
        {
            Ram = new int[OutputHeight, OutputWidth];

            Bitmap = new Bitmap(OutputWidth, OutputHeight, PixelFormat.Format24bppRgb);
        }

        private static readonly int[] MonoBitMask = {0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01};

        public static void DisplayMono8X8(int xCol, int yCol, byte[] monoData8X8, int bgColor, int fgColor)
        {
            var pX = xCol * 8;

            for (var yStep = 0; yStep < 8; yStep++)
            {
                var pY = (yCol * 8) + yStep;
                int value = monoData8X8[yStep];

                for (int i = 0; i < 8; i++)
                {
                    Ram[pY, pX + i] = (value & MonoBitMask[i]) != 0 ? fgColor : bgColor;
                    SetVidPixel(pX + i, pY, Ram[pY, pX + i]);
                }
            }
        }

        public static void SetEgaPalette(int index, int colour16)
        {
            EgaColours[index] = OrigEgaColors[colour16];

            for (var y = 0; y < OutputHeight; y++)
            {
                for (var x = 0; x < OutputWidth; x++)
                {
                    var egaColor = Ram[y, x];
                    var colour = EgaColours[egaColor];
                    _videoRam.SetPixel(x, y, colour);
                }
            }

            Display.Update();
        }

        private static void SetVidPixel(int x, int y, int egaColour)
        {
            var colour = EgaColours[egaColour];
            _videoRam.SetPixel(x, y, colour);
        }

        private static int _noUpdateCount;

        public static void UpdateStop()
        {
            _noUpdateCount++;
        }

        public static void UpdateStart()
        {
            _noUpdateCount--;
            Update();
        }

        public static void Update()
        {
            if (_noUpdateCount != 0) return;

            Bitmap = _videoRam;

            _updateCallback?.Invoke();
        }

        public static void ForceUpdate()
        {
            Bitmap = _videoRam;
            _updateCallback?.Invoke();
        }

        public static void SaveVidRam()
        {
            _videoRamBkUp = (Bitmap) _videoRam.Clone();
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
        }
    }
}