using System;
using System.Collections.Generic;
using System.Text;

namespace Pentris
{
    public class Board
    {
        public Field[,] Fields;

        public Board()
        {
            Fields = new Field[Params.columnCount, Params.rowCount];
        }
    }

    public struct Field
    {
        public bool isOccupied;
        public int colorIndex;
    }
}
