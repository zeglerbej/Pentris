using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pentris
{
    public static class Helpers
    {
        public static bool IsInBounds(int x, int y)
        {
            if (x < 0 || x >= Params.columnCount) return false;
            if (y < 0 || y >= Params.rowCount) return false;
            return true;
        }
        public static bool IsInBounds(Point p)
        {
            if (p.X < 0 || p.X >= Params.columnCount) return false;
            if (p.Y < 0 || p.Y >= Params.rowCount) return false;
            return true;
        }
    }
}
