using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Pentris
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    public abstract class Piece
    {
        protected Point middle;
        protected Direction direction;

        public abstract void Rotate(Board board, Direction rotationDirection);

        public abstract List<Point> GetOccupiedSquares();

        public abstract bool CanRotate(Board board, Direction rotationDirection);

        public abstract bool CanDrop(Board board);

        public abstract bool CanMove(Board board, Direction direction);

        public abstract List<Point> GetNextBoxSquares();

        public void Drop(Board board)
        {
            if (CanDrop(board)) ++middle.Y;
        }

        public void Move(Board board, Direction direction)
        {
            if (direction == Direction.Down || direction == Direction.Up) return;
            if (!CanMove(board, direction)) return;
            if (direction == Direction.Left)
                --middle.X;
            if (direction == Direction.Right)
                ++middle.X;
        }
    }
}
