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
        private Graphics graphics2;
        private Piece currentPiece;
        private Piece nextPiece;
        private Timer timer;
        private Board board;
        private int frameCounter;
        private int squareSizePixel;
        private int nextBoxSquareSizePixel;
        private DASStatus dasStatus;
        private int lastMoveFrame;
        private Direction moveDirection;
        private int dropRate;
        private bool isGameOn;
        private bool showLineDisappearAnimation;
        private int animationStartFrame;
        private List<int> completedLinesIndices;
        private int columnToEraseIndicator;
        private int score;
        private int level;
        private int lines;
        private bool transitioned; 

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
            int x = Math.Abs(this.Width - pictureBox1.Width) / 2;
            int y = Math.Abs(this.Height - pictureBox1.Height) / 2;
            pictureBox1.Location = new Point(x, y);
            int x2 = x + pictureBox1.Width + this.Width / 10;
            pictureBox2.Location = new Point(x2, y);
            nextBoxSquareSizePixel = Params.nextPiecesquareSizePercent * this.Width / 100;
            pictureBox2.Width = nextBoxSquareSizePixel * 7;
            pictureBox2.Height = nextBoxSquareSizePixel * 7;
            button1.Location = new Point(x2, this.Height / 2);
            label4.Location = new Point(x2, this.Height / 2 + button1.Height);
            numericUpDown1.Location = new Point(x2, this.Height / 2 + button1.Height +
                label4.Height);

            int x3 = this.Width / 50;
            label1.Location = new Point(x3, y);
            label2.Location = new Point(x3, y + label1.Height);
            label3.Location = new Point(x3, y + label1.Height + label2.Height);
        }

        private void StartGame(object sender, EventArgs e)
        {
            isGameOn = true;
            ToggleSettingsControl(false);
            Reset();
        }
        private void Reset()
        {
            pictureBox1.BackColor = Params.backgroundColor;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox2.Image = new Bitmap(pictureBox2.Width, pictureBox2.Height);

            graphics = Graphics.FromImage(pictureBox1.Image);
            graphics2 = Graphics.FromImage(pictureBox2.Image);

            if (timer != null) timer.Stop();
            timer = new Timer();
            timer.Interval = 1000 / Params.framesPerSecond;
            timer.Tick += CalculateNextFrame;
            timer.Start();

            currentPiece = null;
            nextPiece = null;

            board = new Board();
            frameCounter = 1;
            dasStatus = DASStatus.Unloaded;
            lastMoveFrame = 0;
            

            showLineDisappearAnimation = false;
            animationStartFrame = 0;
            completedLinesIndices = null;
            columnToEraseIndicator = 0;

            SetResultAndLabels();
            dropRate = Params.dropRate[level];
            Render();
        }

        private void SetResultAndLabels()
        {
            score = 0;
            lines = 0;
            level = (int)numericUpDown1.Value;
            label1.Text = "Score: 0";
            label2.Text = "Lines: 0";
            label3.Text = "Level: " + level.ToString();
        }

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (!isGameOn) return;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    dasStatus = DASStatus.Unloaded;
                    break;
                case Keys.Right:
                    dasStatus = DASStatus.Unloaded;
                    break;
                case Keys.Down:
                    dropRate = Params.dropRate[level];
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!isGameOn) return false;
            if (currentPiece == null) return false;
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
            if (!isGameOn)
            {
                if(timer != null) timer.Stop();
                return;
            }
            if (showLineDisappearAnimation)
            {
                LineDisappearAnimation();
                ++frameCounter;
                Render();
                if(columnToEraseIndicator >= Params.columnCount / 2)
                {
                    showLineDisappearAnimation = false;
                    board.LowerRemainingRows(completedLinesIndices);
                    IncreaseScore(completedLinesIndices.Count);
                    int previousLines = lines;
                    lines += completedLinesIndices.Count;
                    if (lines / 10 > previousLines / 10) Transition();
                    RefreshLabels();
                }
                return;
            }
            if (currentPiece == null)
            {
                if (nextPiece == null) nextPiece = GetRandomPiece();
                currentPiece = nextPiece;
                nextPiece = GetRandomPiece();               
            }
            if (frameCounter % dropRate == 0) DropCurrentPiece();                               
            MakeMove();
            ++frameCounter;
            Render();
        }

        private void LineDisappearAnimation()
        {
            if ((frameCounter - animationStartFrame) % Params.animationFramesPerColumn == 0)
            {
                foreach (int row in completedLinesIndices)
                {
                    int column = Params.columnCount / 2 - 1 - columnToEraseIndicator;
                    board.Fields[column, row].isOccupied = false;
                    column = Params.columnCount / 2 + columnToEraseIndicator;
                    board.Fields[column, row].isOccupied = false;
                    
                }
                ++columnToEraseIndicator;
            }
        }

        private void IncreaseScore(int numberOfLinesCleared)
        {
            int scoreToAdd = 20 * (level + 1);
            switch (numberOfLinesCleared)
            {
                case 1:
                    scoreToAdd *= 2;
                    break;
                case 2:
                    scoreToAdd *= 5;
                    break;
                case 3:
                    scoreToAdd *= 15;
                    break;
                case 4:
                    scoreToAdd *= 60;
                    break;
                case 5:
                    scoreToAdd *= 300;
                    break;
            }
            score += scoreToAdd;            
        }
        private void RefreshLabels()
        {
            label1.Text = "Score: " + score.ToString();
            label1.Refresh();
            label2.Text = "Lines: " + lines.ToString();
            label2.Refresh();
            label3.Text = "Level: " + level.ToString();
            label3.Refresh();
        }
        private void MakeMove()
        {
            if (!isGameOn) return;
            if (dasStatus == DASStatus.Unloaded) return;
            int delay = dasStatus == DASStatus.Delay ? Params.dasDelay : Params.dasNormalRate;
            if(frameCounter - lastMoveFrame == delay)
            {
                dasStatus = DASStatus.Loaded;
                currentPiece.Move(board, moveDirection);
                lastMoveFrame = frameCounter;
            }
        }
        public Piece GetRandomPiece()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int type = rand.Next(Params.PieceTypeCount);
            switch (type)
            {
                case 0:
                    return new LongPiece();
                case 1:
                    return new CrossPiece();
                default:
                    return null;
            }
        }

        private void DropCurrentPiece()
        {
            if (!currentPiece.CanDrop(board))
            {
                var currentPieceSquares = currentPiece.GetOccupiedSquares();
                foreach (Point square in currentPieceSquares)
                {
                    if (!Helpers.IsInBounds(square.X, square.Y)) continue;
                    board.Fields[square.X, square.Y].isOccupied = true;
                }
                currentPiece = nextPiece; 
                nextPiece = GetRandomPiece();
                CheckCompletedLines();
                CheckEnd();
            }
            else currentPiece.Drop(board);
        }

        private void CheckCompletedLines()
        {
            completedLinesIndices = board.GetCompletedLineIndices();
            if(completedLinesIndices.Count > 0)
            {
                showLineDisappearAnimation = true;            
                animationStartFrame = frameCounter;
                columnToEraseIndicator = 0;
            }
        }
        private void CheckEnd()
        {
            var squaresOccupied = currentPiece.GetOccupiedSquares();
            foreach (Point square in squaresOccupied)
            {
                if (!Helpers.IsInBounds(square.X, square.Y)) continue;
                if (board.Fields[square.X, square.Y].isOccupied)
                {
                    isGameOn = false;
                    ToggleSettingsControl(true);
                    if (timer != null)
                    {
                        timer.Stop();
                        timer = null;
                    }
                    break;
                }
            }
        }

        private void Transition()
        {
            if(transitioned)
            {
                ++level;
                dropRate = Params.dropRate[level];
                return;
            }
            int startLevel = (int)numericUpDown1.Value;
            int cap = Math.Max(100, startLevel * 10 - 50);
            int normalTransition = startLevel * 10 + 10;
            int transitionLines = Math.Min(normalTransition, cap);
            if(lines >= transitionLines)
            {
                transitioned = true;
                ++level;
                dropRate = Params.dropRate[level];
            }
        }

        private void ToggleSettingsControl(bool mode)
        {
            numericUpDown1.Enabled = mode;
            numericUpDown1.Visible = mode;
            label4.Visible = mode;
            button1.Visible = mode;
        }

        #endregion
        #region Rendering
        private void Render()
        {
            graphics.Clear(Params.backgroundColor);
            graphics2.Clear(Params.backgroundColor);
            RenderBoard();
            RenderCurrentPiece();
            RenderGrid();
            RenderNextPiece();
            pictureBox1.Refresh();
            pictureBox2.Refresh();
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

        private void RenderNextPiece()
        {
            if (nextPiece == null) return;
            var squaresToFill = nextPiece.GetNextBoxSquares();
            SolidBrush brush = new SolidBrush(Params.occupiedSquareColor);
            foreach (Point square in squaresToFill)
            {
                int x = square.X * nextBoxSquareSizePixel;
                int y = square.Y * nextBoxSquareSizePixel;
                graphics2.FillRectangle(brush, x, y, nextBoxSquareSizePixel, nextBoxSquareSizePixel);
            }
        }
        #endregion
    }
}
