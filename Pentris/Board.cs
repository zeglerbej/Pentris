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

        public List<int> GetCompletedLineIndices()
        {
            List<int> completedLines = new List<int>();
            for(int i = Params.rowCount - 1; i >= 0; --i)
            {
                bool isCompleted = true;
                for(int j = 0; j < Params.columnCount; ++j)
                {
                    if(!Fields[j,i].isOccupied)
                    {
                        isCompleted = false;
                        break;
                    }
                }
                if (isCompleted) completedLines.Add(i);
            }
            return completedLines;
        }

        public void LowerRemainingRows(List<int> completedLinesIndices)
        {
            for(int i = completedLinesIndices.Count - 1; i >= 0; --i)
                LowerBoardByOneRow(completedLinesIndices[i]);
        }

        private void LowerBoardByOneRow(int ind)
        {
            for(int i = ind; i > 0; --i)
            {
                for(int j = 0; j < Params.columnCount; ++j)
                {
                    Fields[j, i] = Fields[j, i - 1];
                }
            }
        }
    }

    public struct Field
    {
        public bool isOccupied;
        public int colorIndex;
    }
}
