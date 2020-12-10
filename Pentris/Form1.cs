using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pentris
{
    public enum DASStatus
    {
        Unloaded,
        Delay,
        Loaded
    }
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private Piece currentPiece;
        private Timer timer;
        private Board board;
        private int frameCounter;
        private int squareSizePixel;
        private DASStatus dasStatus;
        private int lastMoveFrame;
        private Direction moveDirection;
        private int dropRate;

        #region Initialization
        public Form1()
        {
            InitializeComponent();
            AdjustWindow();
            button1.Click += StartGame;
            KeyUp += KeyUpHandler;
            //KeyDown += KeyDownHandler;
            PreviewKeyDown += KeyDownHandler;
            KeyPreview = true;                      
        }

        private void AdjustWindow()
        {
            WindowState = FormWindowState.Maximized;
            squareSizePixel = Params.squareSizePercent * this.Width / 100;
            pictureBox1.Width = squareSizePixel * Params.columnCount;
            pictureBox1.Height = squareSizePixel * Params.rowCount;
            int x = (this.Width - pictureBox1.Width) / 2;
            int y = (this.Height - pictureBox1.Height) / 2;
            pictureBox1.Location = new Point(x, y);
        }

        private void StartGame(object sender, EventArgs e)
        {
            Reset();
        }
        private void Reset()
        {
            pictureBox1.BackColor = Params.backgroundColor;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            
            graphics = Graphics.FromImage(pictureBox1.Image);

            timer = new Timer();
            timer.Interval = 1000 / Params.framesPerSecond;
            timer.Tick += CalculateNextFrame;
            timer.Start();

            currentPiece = null;

            board = new Board();
            frameCounter = 1;
            dasStatus = DASStatus.Unloaded;
            lastMoveFrame = 0;
            dropRate = Params.dropRate;

            Render();
        }

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    dasStatus = DASStatus.Unloaded;
                    break;
                case Keys.Right:
                    dasStatus = DASStatus.Unloaded;
                    break;
                case Keys.Down:
                    dropRate = Params.dropRate;
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Z:
                    currentPiece.Rotate(board, Direction.Left);
                    break;
                case Keys.X:
                    currentPiece.Rotate(board, Direction.Right);
                    break;
                case Keys.Left:
                    if (dasStatus != DASStatus.Unloaded) break;
                    dasStatus = DASStatus.Delay;
                    currentPiece.Move(board, Direction.Left);
                    lastMoveFrame = frameCounter;
                    moveDirection = Direction.Left;
                    return true;
                case Keys.Right:
                    if (dasStatus != DASStatus.Unloaded) break;
                    dasStatus = DASStatus.Delay;
                    currentPiece.Move(board, Direction.Right);
                    lastMoveFrame = frameCounter;
                    moveDirection = Direction.Right;
                    return true;
                case Keys.Down:
                    dropRate = 2;
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void KeyDownHandler(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Z:
                    currentPiece.Rotate(board, Direction.Left);
                    break;
                case Keys.X:
                    currentPiece.Rotate(board, Direction.Right);
                    break;
                case Keys.Left:
                    if (dasStatus != DASStatus.Unloaded) break;
                    dasStatus = DASStatus.Delay;
                    currentPiece.Move(board, Direction.Left);
                    lastMoveFrame = frameCounter;
                    moveDirection = Direction.Left;
                    break;
                case Keys.Right:
                    if (dasStatus != DASStatus.Unloaded) break;
                    dasStatus = DASStatus.Delay;
                    currentPiece.Move(board, Direction.Right);
                    lastMoveFrame = frameCounter;
                    moveDirection = Direction.Right;
                    break;
            }
        }

        #endregion

        #region GameState
        public void CalculateNextFrame(object sender, EventArgs e)
        {
            if (currentPiece == null) GetRandomPiece();
            if (frameCounter % dropRate == 0)
            {
                DropCurrentPiece();                               
            }
            MakeMove();
            ++frameCounter;
            Render();
        }

        private void MakeMove()
        {
            if (dasStatus == DASStatus.Unloaded) return;
            int delay = dasStatus == DASStatus.Delay ? Params.dasDelay : Params.dasNormalRate;
            if(frameCounter - lastMoveFrame == delay)
            {
                dasStatus = DASStatus.Loaded;
                currentPiece.Move(board, moveDirection);
                lastMoveFrame = frameCounter;
            }
        }
        public void GetRandomPiece()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int type = rand.Next(Params.PieceTypeCount);
            switch (type)
            {
                case 0:
                    currentPiece = new LongPiece();
                    break;
            }
        }

        private void DropCurrentPiece()
        {
            if (!currentPiece.CanDrop(board))
            {
                var currentPieceSquares = currentPiece.GetOccupiedSquares();
                foreach (Point square in currentPieceSquares)
                {
                    board.Fields[square.X, square.Y].isOccupied = true;
                }
                GetRandomPiece();
            }
            else currentPiece.Drop(board);
        }

        #endregion
        #region Rendering
        private void Render()
        {
            graphics.Clear(Params.backgroundColor);
            RenderBoard();
            RenderCurrentPiece();
            RenderGrid();
            pictureBox1.Refresh();
        }

        private void RenderBoard()
        {
            SolidBrush brush = new SolidBrush(Params.occupiedSquareColor);
            for (int i = 0; i < Params.columnCount; ++i)
            {
                for(int j = 0; j < Params.rowCount; ++j)
                {
                    if(!board.Fields[i,j].isOccupied) continue;
                    int x = i * squareSizePixel;
                    int y = j * squareSizePixel;
                    graphics.FillRectangle(brush, x, y, squareSizePixel, squareSizePixel);
                }
            }
        }
        private void RenderCurrentPiece()
        {
            if (currentPiece == null) return;
            var currentPieceSquares = currentPiece.GetOccupiedSquares();
            SolidBrush brush = new SolidBrush(Params.occupiedSquareColor);
            foreach (Point square in currentPieceSquares)
            {
                if (!Helpers.IsInBounds(square)) continue;
                int x = square.X * squareSizePixel;
                int y = square.Y * squareSizePixel;
                graphics.FillRectangle(brush, x, y, squareSizePixel, squareSizePixel);
            }
        }
        private void RenderGrid()
        {
            Pen pen = new Pen(Color.White);
            for (int i = 1; i < Params.rowCount; ++i)
            {
                int height = i * squareSizePixel;
                graphics.DrawLine(pen, 0, height, pictureBox1.Width, height);
            }

            for (int i = 1; i < Params.columnCount; ++i)
            {
                int width = i * squareSizePixel;
                graphics.DrawLine(pen, width, 0, width, pictureBox1.Height);
            }
            pen.Dispose();
        }

        #endregion
    }
}
