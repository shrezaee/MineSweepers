using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace MineSweeper
{
    /// <summary>
    /// One of the concepts of the MineSweeper is that each block holds one state
    /// indicates if it's a mine,number(i.e. has adjacent mines) or an empty mine
    /// that has no mines near by.
    /// </summary>
    public enum BlockType { MineBlock,NumberBlock,EmptyBlock,None};

    /// <summary>
    /// This this the abstract class Block that represent all kinds of mines
    /// The initializer of a block determines it's kind by calling the constructor 
    /// of one of the deriven classes of the block class
    /// either MineBlock, NumberBlock or EmptyBlock
    /// </summary>
    abstract class Block
    {
        private bool marked;

        public bool IsMarked
        {
            [DebuggerStepThrough()]
            get { return marked; }
            [DebuggerStepThrough()]
            set { marked = value; }
        }
	
    }

    /// <summary>
    /// A MineBlock is a block holds a mine inside
    /// </summary>
    class MineBlock : Block
    {
        private int rowNo;
        private int colNo;

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
	
        /// <summary>
        /// This the main constructor of the MineBlock which takes 2 values
        /// </summary>
        /// <param name="row">This is the row number of the mine</param>
        /// <param name="col">This is the column number of the mine</param>
        public MineBlock(int row, int col)
        {
            rowNo = row;
            colNo = col;
        }
    }

    /// <summary>
    /// This class represents a Block that has adjacent mines
    /// so it holds also the number of mines that are adjacent
    /// </summary>
    class NumberBlock : Block
    {
        private int neighbourMines;

        public int AdjacentMines
        {
            [DebuggerStepThrough()]
            get { return neighbourMines; }
            [DebuggerStepThrough()]
            set { neighbourMines = value; }
        }
	
        /// <summary>
        /// This is the main constructor of the NumberBlock
        /// </summary>
        /// <param name="mines">The number of adjacent smines</param>
        public NumberBlock(int mines)
        {
            neighbourMines = mines;
        }
        
    }

    /// <summary>
    /// The EmptyBlock class indicates that this Block is empty
    /// i.e. This block is not a mine and doesn't have adjacent mines
    /// </summary>
    class EmptyBlock : Block
    { 
    
    }

    class BlockCollection
    {
        Block[,] currentBlocks;
        int currentWidth;
        int currentHeight;
        MineBlock[] mines;

        public void MarkBlock(int rowNo,int columnNo)
        {
            int dummy = 0;
            if (currentBlocks[rowNo, columnNo] == null)
                ConstructBlock(rowNo, columnNo, ref dummy);
            currentBlocks[rowNo, columnNo].IsMarked = (!currentBlocks[rowNo, columnNo].IsMarked);
        }

        public BlockCollection(int width, int height)
        {
            currentWidth = width;
            currentHeight = height;
            currentBlocks = new Block[height,width];
        }

        private bool GetBlockMarked(int rowNo,int colNo,ref int neighbourMines)
        {
            if (currentBlocks[rowNo, colNo] == null)
                ConstructBlock(rowNo, colNo, ref neighbourMines);
            return currentBlocks[rowNo, colNo].IsMarked;
        }

        public ArrayList GetNonMarkedAdjacentMines(int rowNo,int colNo,ref bool markedMine)
        {
            ArrayList result = new ArrayList();
            int mines = 0;
            for (int i = rowNo - 1; i <= rowNo + 1; i++)
            {
                for (int j = colNo - 1; j <= colNo + 1; j++)
                {
                    try
                    {
                        if (i != rowNo || j != colNo)//in order not to include itself
                        {
                            if (!GetBlockMarked(i, j,ref mines))
                            {
                                
                                if (mines>0)
                                    result.Add(new RowColumnPair(i, j, mines));
                                else
                                    result.Add(new RowColumnPair(i, j));
                                if (GetBlockType(i, j, ref mines) == BlockType.EmptyBlock)
                                    OpenAllAdjacentBlocks(i, j, result);
                                if ((currentBlocks[i, j] as MineBlock) != null)
                                    markedMine = true;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {

                    }
                }
            }
            return result;
        }

        public void OpenAllAdjacentBlocks(int rowNo,int colNumber,ArrayList result)
        {
            result.Add(new RowColumnPair(rowNo, colNumber));
            ArrayList adMines = GetNonConstructedAdjacentMines(rowNo, colNumber);
            RowColumnPair cuPair=null;
            int dummy = 0;
            BlockType cuType;
            for (int i = 0; i < adMines.Count; i++)
            {
                cuPair = (RowColumnPair)(adMines[i]);
                cuType = GetBlockType(cuPair.RowNumber,cuPair.ColumnNumber,ref dummy);
                switch (cuType)
                { 
                    case BlockType.EmptyBlock:
                    case BlockType.NumberBlock:
                        result.Add(cuPair);
                        if (cuType == BlockType.EmptyBlock)
                            OpenAllAdjacentBlocks(cuPair.RowNumber,cuPair.ColumnNumber,result);
                        break;
                }
            }
        }

        private ArrayList GetNonConstructedAdjacentMines(int rowNo, int colNo)
        {
            ArrayList result = new ArrayList();
            int dummy = 0;
            for (int i = rowNo - 1; i <= rowNo + 1; i++)
            {
                for (int j = colNo - 1; j <= colNo + 1; j++)
                {
                    try
                    {
                        if (i != rowNo || j != colNo)//in order not to include itself
                        {
                            if (currentBlocks[i, j] == null)
                            {
                                ConstructBlock(i, j, ref dummy);
                                if(dummy==0) 
                                    result.Add(new RowColumnPair(i,j));
                                else
                                    result.Add(new RowColumnPair(i, j,dummy));
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {

                    }
                }
            }
            return result;
        }

        public ArrayList GetAllMines()
        {
            ArrayList result = new ArrayList(mines.Length);
            for (int i = 0; i < mines.Length; i++)
            {
                result.Add(new RowColumnPair(mines[i].RowNumber,mines[i].ColumnNumber));
            }
            return result;
        }

        public void GenerateMines(int noMines)
        {
            ArrayList arr = new ArrayList();
            Random rand = new Random();
            mines = new MineBlock[noMines];
            int generatedRandom;
            for (int i = 0; i < noMines; i++)
            {
                do
                {
                    generatedRandom = rand.Next(0,(currentWidth*currentHeight)-1);
                }
                while (arr.Contains(generatedRandom));
                arr.Add(generatedRandom);
                mines[i] = new MineBlock(generatedRandom / currentWidth, generatedRandom % currentWidth);
                currentBlocks[generatedRandom / currentWidth, generatedRandom % currentWidth] = mines[i];
            }
        }

        private int CalculateNeibourMines(int row, int column)
        {
            int count = 0;
            MineBlock currentBlock;
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = column - 1; j <= column + 1; j++)
                {
                    try
                    {
                        if (i != row || j != column)//in order not to count itself
                        {
                            currentBlock = currentBlocks[i, j] as MineBlock;
                            if (currentBlock != null)
                                count++;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        
                    }
                }
            }
            return count;
        }

        public int GetConstructedBlocksMinesCount(int rowNo,int colNo)
        {
            try
            {
                if (currentBlocks[rowNo, colNo] == null)
                    return -1;
                else
                    return ((NumberBlock)currentBlocks[rowNo, colNo]).AdjacentMines;
            }
            catch
            {
                return -1;
            }
        }

        public int GetMarkedNeighboursCount(int rowNo,int colNo)
        {
            int count = 0;
            for (int i = rowNo - 1; i <= rowNo + 1; i++)
            {
                for (int j = colNo - 1; j <= colNo + 1; j++)
                {
                    try
                    {
                        if (i != rowNo || j != colNo)//in order not to count itself
                        {
                            if (currentBlocks[i, j] != null)
                            {
                                if(currentBlocks[i, j].IsMarked)                                
                                    count++;    
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {

                    }
                }
            }
            return count;
        }

        public BlockType GetConstructedBlockType(int rowNo, int colNo)
        {
            if ((currentBlocks[rowNo, colNo] as MineBlock) != null)
                return BlockType.MineBlock;
            else if ((currentBlocks[rowNo, colNo] as NumberBlock) != null)
                return BlockType.NumberBlock;
            else if ((currentBlocks[rowNo, colNo] as EmptyBlock) != null)
                return BlockType.EmptyBlock;
            else
                return BlockType.None;
        }

        public BlockType GetBlockType(int rowNo, int colNo, ref int adjBlocks)
        {
            BlockType cuType = GetConstructedBlockType(rowNo, colNo);
            if(cuType==BlockType.NumberBlock)
                return ConstructBlock(rowNo,colNo,ref adjBlocks);
            else if(cuType != BlockType.None)
                return cuType;               
            else
                return ConstructBlock(rowNo, colNo, ref adjBlocks);
        }

        private BlockType ConstructBlock(int rowNo,int columnNo,ref int adjacentBlocks)
        {
            adjacentBlocks = CalculateNeibourMines(rowNo, columnNo);
            if (adjacentBlocks == 0)
            {
                currentBlocks[rowNo, columnNo] = new EmptyBlock();
                return BlockType.EmptyBlock;
            }
            else
            {
                currentBlocks[rowNo, columnNo] = new NumberBlock(adjacentBlocks);
                return BlockType.NumberBlock;
            }
        }
    }
    class RowColumnPair
    {
        public RowColumnPair()
        {
            mines = -1;
        }
        public RowColumnPair(int row,int col):this(row,col,-1)
        {
            
        }
        public RowColumnPair(int row, int col, int mines)
        {
            rowNo = row;
            colNo = col;
            this.mines = mines;
        }

        private int colNo;

        public int ColumnNumber
        {
            get { return colNo; }
            set { colNo = value; }
        }
	
        private int rowNo;

        public int RowNumber
        {
            get { return rowNo; }
            set { rowNo = value; }
        }

        private int mines;

        public int NumberOfMines
        {
            get { return mines; }
            set { mines = value; }
        }
	
	
    }
}
