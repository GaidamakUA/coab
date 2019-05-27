using System;
using System.Collections.Generic;
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
        private static readonly Color[] EgaColors =
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

        private static readonly Dictionary<int, int> EgaColorMap = new Dictionary<int, int>();

        private const int OutputWidth = 320;
        private const int OutputHeight = 200;

        private static Bitmap _bitmapModel = new Bitmap(OutputWidth, OutputHeight, PixelFormat.Format24bppRgb);
        public static Bitmap Bitmap = new Bitmap(OutputWidth, OutputHeight, PixelFormat.Format24bppRgb);
        private static Bitmap _bitmapModelBackUp;

        public delegate void VoidDelegate();

        private static VoidDelegate _updateCallback;

        public static VoidDelegate UpdateCallback
        {
            set => _updateCallback = value;
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
                    var egaColour = (value & MonoBitMask[i]) != 0 ? fgColor : bgColor;
                    SetVidPixel(pX + i, pY, egaColour);
                }
            }
        }

        public static void SetColorMap(int mappedTo, int original)
        {
            if (mappedTo == original)
            {
                EgaColorMap.Remove(original);
            }
            else
            {
                EgaColorMap.Add(original, mappedTo);
            }

            Update();
        }

        private static void SetVidPixel(int x, int y, int egaColour)
        {
            var colour = EgaColors[egaColour];
            _bitmapModel.SetPixel(x, y, colour);
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
            ForceUpdate();
        }

        public static void ForceUpdate()
        {
            if (EgaColorMap.Count == 0)
            {
                Bitmap = _bitmapModel;
            }
            else
            {
                ColorMap[] remapTable = new ColorMap[EgaColorMap.Count];
                var index = 0;
                foreach (var keyValuePair in EgaColorMap)
                {
                    var colorMap = new ColorMap();
                    colorMap.OldColor = EgaColors[keyValuePair.Key];
                    colorMap.NewColor = EgaColors[keyValuePair.Value];
                    remapTable[index++] = colorMap;
                }

                var attributes = new ImageAttributes();
                attributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
                Bitmap = new Bitmap(OutputWidth, OutputHeight, PixelFormat.Format24bppRgb);
                var graphics = Graphics.FromImage(Bitmap);
                graphics.DrawImage(_bitmapModel,
                    new Rectangle(0, 0, OutputWidth, OutputHeight),
                    0,
                    0,
                    OutputWidth,
                    OutputHeight,
                    GraphicsUnit.Pixel,
                    attributes);
                graphics.Dispose();
            }

            _updateCallback?.Invoke();
        }

        public static void SaveVidRam()
        {
            _bitmapModelBackUp = (Bitmap) _bitmapModel.Clone();
        }

        public static void RestoreVidRam()
        {
            _bitmapModel = _bitmapModelBackUp;
        }

        public static byte GetPixel(int x, int y)
        {
            return (byte) Array.IndexOf(EgaColors, _bitmapModel.GetPixel(x, y));
        }

        public static void SetPixel3(int x, int y, int value)
        {
            if (value < 16)
            {
                SetVidPixel(x, y, value);
            }
        }

        public static void ClearRectangle(int x, int y, int width, int height)
        {
            DrawRectangle(x, y, width, height, 0);
        }

        public static void DrawRectangle(int x, int y, int width, int height, int egaColour)
        {
            var colour = EgaColors[egaColour];
            var graphics = Graphics.FromImage(_bitmapModel);
            graphics.FillRectangle(new SolidBrush(colour), x, y, width, height);
            graphics.Dispose();
        }
    }
}