using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pentris
{
    public class LongPiece : Piece
    {
        public LongPiece()
        {
            direction = Direction.Right;
            middle = new Point(Params.columnCount / 2, 0);
        }
        public override bool CanDrop(Board board)
        {
            if(direction == Direction.Right)
            {
                if (middle.Y >= Params.rowCount - 1) return false;
                for (int i = -2; i <= 2; ++i)
                {
                    if (board.Fields[middle.X + i, middle.Y + 1].isOccupied) return false;                   
                }
                return true;
            }
            else
            {
                if (middle.Y >= Params.rowCount - 3) return false;
                if (board.Fields[middle.X, middle.Y + 3].isOccupied) return false;
                return true;
            }
        }

        public override bool CanMove(Board board, Direction moveDirection)
        {
            if (moveDirection == Direction.Down || moveDirection == Direction.Up) return false;
            if(direction == Direction.Right)
            {
                int adjacentBlockX = moveDirection == Direction.Left ? middle.X - 3 : middle.X + 3;
                if (!Helpers.IsInBounds(adjacentBlockX, middle.Y)) return false;
                if (board.Fields[adjacentBlockX, middle.Y].isOccupied) return false;
                return true;
            }
            else
            {
                for (int i = -2; i <= 2; ++i)
                {
                    int adjacentBlockX = moveDirection == Direction.Left ? middle.X - 1 : middle.X + 1;
                    int adjacentBlockY = middle.Y + i;
                    if (!Helpers.IsInBounds(adjacentBlockX, adjacentBlockY))
                    {
                        return false;
                    }
                    if (board.Fields[adjacentBlockX, adjacentBlockY].isOccupied)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public override bool CanRotate(Board board, Direction rotationDirection)
        {
            for (int i = -2; i <= 2; ++i)
            {
                int x = direction == Direction.Right ? middle.X : middle.X + i;
                int y = direction == Direction.Right ? middle.Y + i : middle.Y;
                if (!Helpers.IsInBounds(x, y)) return false;
                if (board.Fields[x, y].isOccupied) return false;
            }
            return true;
        }

        public override List<Point> GetOccupiedSquares()
        {
            List<Point> points = new List<Point>();
            if(direction == Direction.Right)
                for (int i = -2; i <= 2; ++i) points.Add(new Point(middle.X + i, middle.Y));
            else
                for (int i = -2; i <= 2; ++i) points.Add(new Point(middle.X, middle.Y + i));
            return points;
        }

        public override void Rotate(Board board, Direction rotationDirection)
        {
            if (!CanRotate(board, rotationDirection)) return;
            if (direction == Direction.Up) direction = Direction.Right;
            else direction = Direction.Up;
        }
    }
}
