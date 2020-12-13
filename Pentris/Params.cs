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
        public const int nextPiecesquareSizePercent = 2;
        public const int PieceTypeCount = 1;
        public const int dasDelay = 16;
        public const int dasNormalRate = 6;
        public const int animationFramesPerColumn = 2;
        public static readonly Color backgroundColor = Color.Black;
        public static readonly Color occupiedSquareColor = Color.FromArgb(55, 110, 191);

        public static readonly int[] dropRate = new int[]
        { 48, 43, 38, 33, 28, 23, 18, 13, 8, 6, 5, 5, 5, 4, 4, 4, 3, 3, 3,
            2, 2, 2, 2, 2 , 2, 2, 2, 2, 2, 2, 1};
    }
}
