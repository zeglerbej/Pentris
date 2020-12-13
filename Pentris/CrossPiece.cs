using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pentris
{
    public class CrossPiece : Piece
    {
        public CrossPiece()
        {
            direction = Direction.Right;
            middle = new Point(Params.columnCount / 2, 1);
        }
        public override bool CanDrop(Board board)
        {
            if (!Helpers.IsInBounds(middle.X, middle.Y + 2)) return false;
            if (board.Fields[middle.X, middle.Y + 2].isOccupied) return false;
            if (!Helpers.IsInBounds(middle.X - 1, middle.Y + 1)) return false;
            if (board.Fields[middle.X - 1, middle.Y + 1].isOccupied) return false;
            if (!Helpers.IsInBounds(middle.X + 1, middle.Y + 1)) return false;
            if (board.Fields[middle.X + 1, middle.Y + 1].isOccupied) return false;
            return true;
        }

        public override bool CanMove(Board board, Direction direction)
        {
            if(direction == Direction.Left)
            {
                if (!Helpers.IsInBounds(middle.X - 2, middle.Y)) return false;
                if (board.Fields[middle.X - 2, middle.Y].isOccupied) return false;
                if (!Helpers.IsInBounds(middle.X - 1, middle.Y - 1)) return false;
                if (board.Fields[middle.X - 1, middle.Y - 1].isOccupied) return false;
                if (!Helpers.IsInBounds(middle.X - 1, middle.Y + 1)) return false;
                if (board.Fields[middle.X - 1, middle.Y + 1].isOccupied) return false;
            }
            if (direction == Direction.Right)
            {
                if (!Helpers.IsInBounds(middle.X + 2, middle.Y)) return false;
                if (board.Fields[middle.X + 2, middle.Y].isOccupied) return false;
                if (!Helpers.IsInBounds(middle.X + 1, middle.Y - 1)) return false;
                if (board.Fields[middle.X + 1, middle.Y - 1].isOccupied) return false;
                if (!Helpers.IsInBounds(middle.X + 1, middle.Y + 1)) return false;
                if (board.Fields[middle.X + 1, middle.Y + 1].isOccupied) return false;
            }
            return true;
        }

        public override bool CanRotate(Board board, Direction rotationDirection)
        {
            return true;
        }

        public override List<Point> GetNextBoxSquares()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point(3, 3));
            points.Add(new Point(3, 4));
            points.Add(new Point(3, 2));
            points.Add(new Point(2, 3));
            points.Add(new Point(4, 3));
            return points;
        }

        public override List<Point> GetOccupiedSquares()
        {
            List<Point> points = new List<Point>();
            points.Add(middle);
            points.Add(new Point(middle.X - 1, middle.Y));
            points.Add(new Point(middle.X + 1, middle.Y));
            points.Add(new Point(middle.X, middle.Y - 1));
            points.Add(new Point(middle.X, middle.Y + 1));
            return points;
        }

        public override void Rotate(Board board, Direction rotationDirection)
        {
            return;
        }
    }
}
