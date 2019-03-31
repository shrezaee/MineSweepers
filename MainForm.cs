using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

namespace MineSweeper
{
    /// <summary>
    /// The MainForm Class is the main form of the minesweeper game
    /// which displays the game main window
    /// </summary>
    public partial class MainForm : Form
    {
        MineSweeperBLT mineSweeper;
        MineButton[,] currentButtons;
        Timer timer;
        Timer lblTimer;
        int leftRightClick;
        MineButton currentMine;
        bool gameOver;
        ArrayList pairs;

        public MainForm()
        {
            InitializeComponent();
            ConstructMembers();
        }

        void ConstructMembers()
        {
            mineSweeper = new MineSweeperBLT();
            StartNewGame(GameLevel.Beginner);
            Application.DoEvents();
            timer = new Timer();
            lblTimer = new Timer();
            timer.Interval = 100;
            lblTimer.Interval = 100;
            timer.Tick += new EventHandler(timer_Tick);
            lblTimer.Tick += new EventHandler(lblTimer_Tick);
            leftRightClick = 0;
            gameOver = false;
            pairs = new ArrayList();
        }

        void DistroyMembers()
        {
            mineSweeper = null;
            currentButtons = null;
            timer = null;
            lblTimer = null;
            currentMine = null;
            gameOver = false;
            pairs = null;
        }

        void lblTimer_Tick(object sender, EventArgs e)
        {
            lblTimer.Enabled = false;
            HandleButtonClicked(ClickResult.DoNothing, 0);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;
            ClickResult aResult = ClickResult.DoNothing;
            int adjacentBlocks = 0;
            switch (leftRightClick)
            {
                case 1:
                    pairs.Clear();
                    aResult = mineSweeper.LeftClickEvent(currentMine.RowNumber, currentMine.ColumnNumber, ref adjacentBlocks, pairs);
                    break;
                case 2:
                    pairs.Clear();
                    aResult = mineSweeper.RightClickEvent(currentMine.RowNumber, currentMine.ColumnNumber);
                    break;
            }
            leftRightClick = 0;
            HandleButtonClicked(aResult,adjacentBlocks);
        }

        void HandleButtonClicked(ClickResult bResult,int blockNumber)
        {
            switch (bResult)
            {
                case ClickResult.GameOver:
                    GameOver();
                    break;
                case ClickResult.MarkBlock:
                    MarkBlock();
                    break;
                case ClickResult.OpenBlocks:
                    OpenBlocks();
                    break;
                case ClickResult.OpenBlock:
                    OpenBlock(currentMine.RowNumber,currentMine.ColumnNumber,blockNumber);
                    break;
            }
        }

        private void lbl_Click(object sender, MouseEventArgs e)
        {
            if (!gameOver)
            {
                ClickResult aResult = ClickResult.DoNothing;
                NumberedLabel aLabel = (NumberedLabel)sender;
                currentMine = currentButtons[aLabel.RowNumber,aLabel.ColumnNumber];
                if (e.Button == MouseButtons.Left)
                {
                    if (lblTimer.Enabled && leftRightClick == 2)
                    {
                        leftRightClick = 0;
                        pairs.Clear();
                        aResult = mineSweeper.DoubleClickEvent(currentMine.RowNumber, currentMine.ColumnNumber, pairs);
                        lblTimer.Enabled = false;
                        HandleButtonClicked(aResult, -1);
                        return;
                    }
                    leftRightClick = 1;
                    lblTimer.Enabled = true;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (lblTimer.Enabled && leftRightClick == 1)
                    {
                        leftRightClick = 0;
                        pairs.Clear();
                        aResult = mineSweeper.DoubleClickEvent(currentMine.RowNumber, currentMine.ColumnNumber, pairs);
                        lblTimer.Enabled = false;
                        HandleButtonClicked(aResult, -1);
                        return;
                    }
                    leftRightClick = 2;
                    lblTimer.Enabled = true;
                }
            }
        }

        void OpenBlock(int rowNo, int colNo, int mineNos)
        {
            NumberedLabel cuLbl = new NumberedLabel();
            if (mineNos > 0)
                cuLbl.Text = mineNos.ToString();
            cuLbl.Location = currentButtons[rowNo, colNo].Location;
            cuLbl.Name = "Lbl" + rowNo.ToString() + colNo.ToString();
            cuLbl.Size = new System.Drawing.Size(20, 20);
            cuLbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            cuLbl.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cuLbl.RowNumber = rowNo;
            cuLbl.ColumnNumber = colNo;
            cuLbl.Number = mineNos;
            cuLbl.MouseDown += new MouseEventHandler(this.lbl_Click);
            switch (mineNos)
            { 
                case 1:
                    cuLbl.ForeColor = System.Drawing.Color.Navy;
                    break;
                case 2:
                    cuLbl.ForeColor = System.Drawing.Color.DarkGreen;
                    break;
                case 3:
                    cuLbl.ForeColor = System.Drawing.Color.DarkRed;
                    break;
                case 4:
                    cuLbl.ForeColor = System.Drawing.Color.Lime;
                    break;
                case 5:
                    cuLbl.ForeColor = System.Drawing.Color.CornflowerBlue;
                    break;
                case 6:
                    cuLbl.ForeColor = System.Drawing.Color.DeepPink;
                    break;
                case 7:
                    cuLbl.ForeColor = System.Drawing.Color.SlateBlue;
                    break;
                case 8:
                    cuLbl.ForeColor = System.Drawing.Color.Violet;
                    break;
            }
            
            this.Controls.Add(cuLbl);
            cuLbl.Show();
            currentButtons[rowNo, colNo].Hide();
        }

        void GameOver()
        {
            gameOver = true;
            pairs.Clear();
            pairs = mineSweeper.GetAllMines();
            ShowMines();
            MessageBox.Show("You Lose, Game Over");
        }

        void ShowMines()
        {
            foreach (RowColumnPair pair in pairs)
            {
                ShowMine(pair.RowNumber,pair.ColumnNumber);
            }
        }

        void ShowMine(int rowNo, int colNo)
        {
            Label cuLbl = new Label();
            cuLbl.Location = currentButtons[rowNo, colNo].Location;
            cuLbl.Name = "Lbl" + rowNo.ToString() + colNo.ToString();
            cuLbl.Size = new System.Drawing.Size(20, 20);
            this.Controls.Add(cuLbl);
            cuLbl.Paint += new PaintEventHandler(this.Label_Paint);
            cuLbl.Show();
            currentButtons[rowNo, colNo].Hide();
        }

        void MarkBlock()
        {
            bool mark = (!currentMine.IsMarked);
            currentMine.IsMarked = mark;
            if (mark)
            {
                currentMine.Paint += new PaintEventHandler(this.button_Paint);
                currentMine.Refresh();
            }
            else
            {
                currentMine.Paint -= new PaintEventHandler(this.button_Paint);
                currentMine.Refresh();
            }
        }

        void OpenBlocks()
        {
            if (pairs != null)
            {
                foreach (RowColumnPair pair in pairs)
                {
                    OpenBlock(pair.RowNumber,pair.ColumnNumber,pair.NumberOfMines);
                }
            }
        }

        void DrawFlag(int row,int column)
        { 
            
        }

        /// <summary>
        /// Initiation of the button arrary that represents the blocks of the game
        /// </summary>
        private void InitiateButtons()
        {
            currentButtons = new MineButton[mineSweeper.CurrentHeight, mineSweeper.CurrentWidth];
        }

        /// <summary>
        /// Continue of the initialization of each button and set each button attributes, drawing and event handling
        /// </summary>
        private void DrawButtons()
        {
            Point currentPoint;
            for (int i = 0; i < mineSweeper.CurrentHeight; i++)
            {
                for (int j = 0; j < mineSweeper.CurrentWidth; j++)
                {
                    currentButtons[i, j] = new MineButton();
                    currentButtons[i, j].Width = 20;
                    currentButtons[i, j].Height = 20;
                    currentPoint = new Point(j* 20, i* 20+25);
                    currentButtons[i, j].Location = currentPoint;
                    currentButtons[i, j].UseVisualStyleBackColor = true;
                    this.Controls.Add(currentButtons[i, j]);
                    currentButtons[i, j].RowNumber = i;
                    currentButtons[i, j].ColumnNumber = j;
                    currentButtons[i, j].MouseDown += new MouseEventHandler(this.button_Click);
                    
                }
            }
            this.Width = mineSweeper.CurrentWidth * 20+10;
            this.Height = mineSweeper.CurrentHeight * 20 + 60;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void button_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Image.FromFile(@"flag.JPG"), 3, 4);
        }

        private void Label_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Image.FromFile(@"mine.JPG"), 1, 2);
        }

        /// <summary>
        /// The event handle of start new game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void beginnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameLevel currentLevel;
            ToolStripMenuItem aItem = (ToolStripMenuItem)sender;
            if (aItem == beginnerToolStripMenuItem)
                currentLevel = GameLevel.Beginner;
            else if (aItem == intermediateToolStripMenuItem)
                currentLevel = GameLevel.Intermediate;
            else
                currentLevel = GameLevel.Advanced;
            this.Controls.Clear();
            //DistroyMembers();
            InitializeComponent();
            //ConstructMembers();            
            StartNewGame(currentLevel);
        }

        /// <summary>
        /// Initialization of a new game
        /// </summary>
        /// <param name="currentLevel">game level to start with</param>
        private void StartNewGame(GameLevel currentLevel)
        {
            mineSweeper = new MineSweeperBLT();
            mineSweeper.StartGame(currentLevel);
            InitiateButtons();
            DrawButtons();
            gameOver = false;
        }

        /// <summary>
        /// Handling the event of clicking on a mine
        /// whether it's a right, left or both like in the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, MouseEventArgs e)
        {
            if (!gameOver)
            {
                currentMine = (MineButton)sender;
                if (e.Button == MouseButtons.Left)
                {
                    leftRightClick = 1;
                    timer.Enabled = true;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    leftRightClick = 2;
                    timer.Enabled = true;
                }
            }
        }

      
    }

    /// <summary>
    /// The MineButton Class is a simple button but this button holds his row/column numbers
    /// </summary>
    [DebuggerDisplay("Row = {rowNo}, Column = {colNo}, IsMarked = {marked}")]
    class MineButton : Button
    {
        private int rowNo;
        private int colNo;
        private bool marked;

        public bool IsMarked
        {
            [DebuggerStepThrough()]
            get { return marked; }
            [DebuggerStepThrough()]
            set { marked = value; }
        }
	

        public int ColumnNumber
        {
            [DebuggerStepThrough()]
            get { return colNo; }
            [DebuggerStepThrough()]
            set { colNo = value; }
        }
	

        public int RowNumber
        {
            [DebuggerStepThrough()]
            get { return rowNo; }
            [DebuggerStepThrough()]
            set { rowNo = value; }
        }
	
    }

    [DebuggerDisplay("Row = {rowNo}, Column = {colNo}, Number = {number}")]
    class NumberedLabel : Label
    {
        private int rowNo;
        private int colNo;
        private int number;

        public int Number
        {
            [DebuggerStepThrough()]
            get { return number; }
            [DebuggerStepThrough()]
            set { number = value; }
        }
	

        public int ColumnNumber
        {
            [DebuggerStepThrough()]
            get { return colNo; }
            [DebuggerStepThrough()]
            set { colNo = value; }
        }
	

        public int RowNumber
        {
            [DebuggerStepThrough()]
            get { return rowNo; }
            [DebuggerStepThrough()]
            set { rowNo = value; }
        }
	
    }
}