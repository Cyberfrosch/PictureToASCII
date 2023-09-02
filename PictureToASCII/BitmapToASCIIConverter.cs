using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.AccessControl;

namespace PictureToASCII
{
    public class BitmapToASCIIConverter
    {
        private static readonly char[] _asciiTable = { '.', ',', ':', '+', '*', '?', '%', 'S', '#', '@' };
        private static readonly char[] _asciiTableReverse = { '@', '#', 'S', '%', '?', '*', '+', ':', ',', '.' };
        private static readonly int _asciiTableSize = _asciiTable.Length - 1;
        private Bitmap _bitmap;

        public BitmapToASCIIConverter(Bitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public char[][] Convert()
        {
            return Convert(_asciiTable);
        }

        public char[][] ConvertReverse()
        {
            return Convert(_asciiTableReverse);
        }

        public char[][] Convert(char[] asciiTable)
        {
            var result = new char[_bitmap.Height][];

            for (int y = 0; y < _bitmap.Height; y++)
            {
                result[y] = new char[_bitmap.Width];
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    //TODO: cast Color to Double
                    int mapIndex = (int)Map(_bitmap.GetPixel(x, y).R, 0, 255, 0, _asciiTableSize);
                    result[y][x] = asciiTable[mapIndex];
                }
            }

            return result;
        }

        private double Map(double valueToMap, int firstOriginal, int lastOriginal, int firstNew, int lastNew) 
        {
            return (valueToMap - firstOriginal) / (lastOriginal - firstOriginal) * (lastNew - firstNew) + firstNew;
        }
    }
}
