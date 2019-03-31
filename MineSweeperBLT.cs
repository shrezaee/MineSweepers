using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace MineSweeper
{
    public enum GameLevel{Beginner,Intermediate,Advanced};
    public enum ClickResult { OpenBlocks,OpenBlock,GameOver,DoNothing,MarkBlock,YouWin};

    /// <summary>
    /// The MineSweeperBLT is the class that performs the Bussiness Logic of the game
    /// </summary>
    class MineSweeperBLT
    {
        BlockCollection blocks;
        const int BeginnerWidth = 9;
        const int BeginnerHeight = 9;
        const int IntermediateWidth = 16;
        const int IntermediateHeight = 16;
        const int AdvancedWidth = 30;
        const int AdvancedHeight = 16;
        const int noMinesBeginner = 10;
        const int noMinesIntermediate = 40;
        const int noMinesAdvanced = 99;
        GameLevel aLevel;
        private int currentGameWidth;
        private int currentGameHeight;

        /// <summary>
        /// The CurrentHeight property represents the number of cells of the current game
        /// In other words it represents the total number of rows
        /// </summary>
        public int CurrentHeight
        {
            [DebuggerStepThrough()]
            get { return currentGameHeight; }
            [DebuggerStepThrough()]
            set { currentGameHeight = value; }
        }

        /// <summary>
        /// The CurrentWidth property represents the number of cells of the current game
        /// In other words it represents the total number of columns
        /// </summary>
        public int CurrentWidth
        {
            [DebuggerStepThrough()]
            get { return currentGameWidth; }
            [DebuggerStepThrough()]
            set { currentGameWidth = value; }
        }

        /// <summary>
        /// The CurrentLevel property represents the level of the current game
        /// either beginner, intermediate or Advanced
        /// </summary>
        public GameLevel CurrentLevel
        {
            [DebuggerStepThrough()]
            get { return aLevel; }
            [DebuggerStepThrough()]
            set { aLevel = value; }
        }
	
        /// <summary>
        /// The default constructor of the BLT class
        /// which does nothing
        /// </summary>
        public MineSweeperBLT()
        {
            
        }

        /// <summary>
        /// The StartGame method handles the Start Game event
        /// which initializes a new game of a specific level
        /// </summary>
        /// <param name="currentGame">The game level which we want to start the game with</param>
        public void StartGame(GameLevel currentGame)
        {
            aLevel = currentGame;
            InitiateBlocks();
            GenerateBlocks();
        }

        /// <summary>
        /// The GetAllMines method returns the locations of all mines
        /// in order to show them if game is over
        /// </summary>
        /// <returns>All Mines Locations as arraylist of RowColumnPair</returns>
        public ArrayList GetAllMines()
        {
            return blocks.GetAllMines();
        }

        /// <summary>
        /// The LeftClickEvent method handles the left click on any block in the minesweeper board
        /// </summary>
        /// <param name="rowNo">The row number which has been clicked</param>
        /// <param name="colNo">The column number which has been clicked</param>
        public ClickResult LeftClickEvent(int rowNo, int colNo,ref int adjacentBlocks,ArrayList pairs)
        {
            BlockType aType = blocks.GetBlockType(rowNo,colNo,ref adjacentBlocks);
            switch (aType)
            {
                case BlockType.EmptyBlock:
                    blocks.OpenAllAdjacentBlocks(rowNo, colNo, pairs);
                    return ClickResult.OpenBlocks;
                case BlockType.MineBlock:
                    return ClickResult.GameOver;
                case BlockType.NumberBlock:
                    return ClickResult.OpenBlock;
            }
            return ClickResult.DoNothing;
        }

        /// <summary>
        /// This method handles the right/left click at the same time
        /// </summary>
        /// <param name="rowNo">the row number of the click</param>
        /// <param name="colNo">the column number of the click</param>
        /// <param name="pairs">the blocks to open</param>
        /// <returns></returns>
        public ClickResult DoubleClickEvent(int rowNo, int colNo, ArrayList pairs)
        {
            BlockType cuType = blocks.GetConstructedBlockType(rowNo, colNo);
            if(cuType == BlockType.NumberBlock)
            {
                int mines = blocks.GetConstructedBlocksMinesCount(rowNo,colNo);
                int adjacentBlocksMarked = blocks.GetMarkedNeighboursCount(rowNo, colNo);
                if (mines == adjacentBlocksMarked)
                {
                    bool gameOver = false;
                    ArrayList tmpList = blocks.GetNonMarkedAdjacentMines(rowNo, colNo, ref gameOver);
                    if (gameOver)
                        return ClickResult.GameOver;
                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        pairs.Add((RowColumnPair)tmpList[i]);
                    }
                    return ClickResult.OpenBlocks;
                }
            }
            return ClickResult.DoNothing;
        }

        /// <summary>
        /// This method handles the right click event on any block
        /// </summary>
        /// <param name="rowNo">the row number of the click</param>
        /// <param name="colNo">the column number of the click</param>
        /// <returns>reurns MarkBlock ClickResult</returns>
        public ClickResult RightClickEvent(int rowNo, int colNo)
        {
            blocks.MarkBlock(rowNo, colNo);
            return ClickResult.MarkBlock;
        }

        /// <summary>
        /// This method initiate the blocks at the beginning of the game
        /// </summary>
        private void InitiateBlocks()
        {
            switch (aLevel)
            {
                case GameLevel.Beginner:
                    blocks = new BlockCollection(BeginnerWidth, BeginnerHeight);
                    currentGameHeight = BeginnerHeight;
                    currentGameWidth = BeginnerWidth;
                    break;
                case GameLevel.Intermediate:
                    blocks = new BlockCollection(IntermediateWidth, IntermediateHeight);
                    currentGameHeight = IntermediateHeight;
                    currentGameWidth = IntermediateWidth;
                    break;
                case GameLevel.Advanced:
                    blocks = new BlockCollection(AdvancedWidth, AdvancedHeight);
                    currentGameHeight = AdvancedHeight;
                    currentGameWidth = AdvancedWidth;
                    break;
            }
        }

        /// <summary>
        /// This method generates the Mines at the beginning of the game
        /// depending on the game level
        /// </summary>
        private void GenerateBlocks()
        {
            switch (aLevel)
            {
                case GameLevel.Beginner:
                    blocks.GenerateMines(noMinesBeginner);
                    break;
                case GameLevel.Intermediate:
                    blocks.GenerateMines(noMinesIntermediate);
                    break;
                case GameLevel.Advanced:
                    blocks.GenerateMines(noMinesAdvanced);
                    break;
            }
        }
    }
}
