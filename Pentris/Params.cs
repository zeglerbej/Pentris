using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pentris
{
    public static class Params
    {
        public const int rowCount = 21;
        public const int columnCount = 12;
        public const int framesPerSecond = 60;
        public const int squareSizePercent = 3;
        public const int PieceTypeCount = 1;
        public const int dropRate = 50;
        public const int dasDelay = 16;
        public const int dasNormalRate = 6;
        public static readonly Color backgroundColor = Color.Black;
        public static readonly Color occupiedSquareColor = Color.FromArgb(55, 110, 191);
    }
}
