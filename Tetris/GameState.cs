namespace Tetris
{
    public class GameState
    {
        //Attributes
        private Block _currentBlock;                    //The currently moving block
        private Block _heldBlock;                       //The current block on hold
        private bool _canHold;                          
        private GameGrid _gameGrid;                     //The main game grid
        private BlockQueue _blockQueue;                 //The main block queue
        private bool _gameOver;                         //
        private int _score;                             //Score
        

        //Properties
        public Block CurrentBlock
        {
            get => _currentBlock;
            set
            {
                _currentBlock = value;
                _currentBlock.Reset(); //Set the correct start position and rotation state when initializing the current block

                for (int i = 0; i <2; i++) // Move the current block down two rows if there's no collision
                {
                    _currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        _currentBlock.Move(-1, 0);
                    }
                }
            }
        }

        public Block HeldBlock
        {
            get => _heldBlock;
            set => _heldBlock = value;  
        }

        public bool CanHold
        {
            get => _canHold;
            set => _canHold = value;            
        }

        public GameGrid GameGrid
        {
            get => _gameGrid;
            set => _gameGrid = value;
        }

        public BlockQueue BlockQueue
        {
            get => _blockQueue;
            set => _blockQueue = value;
        }

        public bool GameOver
        {
            get => _gameOver;
            set => _gameOver = value;
        }

        public int Score
        {
            get => _score;
            set=> _score = value;
        }

        //Constructor
        public GameState()
        {
            GameGrid = new GameGrid(22, 10);                
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
            CanHold = true;
        }

        //Methods
        private bool BlockFits() //Check if the current block is in an illegal position
        {
            foreach (Position p in CurrentBlock.TilePositions()) //Loop through every tiles' position in the current block
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column))          //If any of the position is outside the grid or overlap with another block, return false
                {
                    return false;
                }
            }
            return true; 
        }

        public void HoldBlock() //Hold a block
        {
            if (!CanHold)
            {
                return;                                  //Avoid a recently released block from being held again
            }

            if (HeldBlock == null)                      //If there's no block on hold
            {
                HeldBlock = CurrentBlock;                   //Hold the current block
                CurrentBlock = BlockQueue.NextBlock;        //Get the next block
            }
            else                                        //Otherwise, swap the current block with the held block
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }

            CanHold = false;
        }

        public void RotateBlockCW() //Rotate the current block clockwise
        {
            CurrentBlock.RotateCW();

            if (!BlockFits()) //If rotating results in an illegal position, rotates it back
            {
                CurrentBlock.RotateCCW();
            }
        }

        public void RotateBlockCCW() //Rotate the current block counter-clockwise
        {
            CurrentBlock.RotateCCW();

            if (!BlockFits()) //If rotating results in an illegal position, rotates back
            {
                CurrentBlock.RotateCW();
            }
        }

        public void MoveBlockLeft() //Move the block 1 column to the left
        {
            CurrentBlock.Move(0, -1); 

            if (!BlockFits()) //If moving results in an illegal position, moves back
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight() //Move the block 1 column to the right
        {
            CurrentBlock.Move(0, 1);

                if (!BlockFits()) //If moving results in an illegal position, moves back
                {
                    CurrentBlock.Move(0, -1);
                }
        }

        public bool IsGameOver() //Check if the game is over
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1)); //If neither of the top two rows are empty, game over!
        }

        public void PlaceBlock()                                    //Place a block onto the grid
        {
            foreach (Position p in CurrentBlock.TilePositions())    //Loop through the tiles' position in the current block
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;        //Set the respective positions in the game grid to the block Id
            }

            Score += GameGrid.ClearFullRows();                      //Clear any full rows and increment the score by that value

            if (IsGameOver())                                       //If the game is over
            {
                GameOver = true;                                    //Set the GameOver property to true
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();           //Else update the current block
                CanHold = true;                                     //Allow the next block to be held
            }
        }

        public void MoveBlockDown() //Move the current block down 1 row
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())                                       //If moving the block down results in an illegal state
            {
                CurrentBlock.Move(-1, 0);                           //Move the block up 
                PlaceBlock();                                       //Place the block onto the grid
            }
        }

        public int TileDropDistance(Position p)                     //Return the number of empty cells below it
        {
            int drop = 0;

            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p)); //Set the drop distance to the minimum value among all the tiles in the block
            }

            return drop;
        }

        public void DropBlock()                                     //Hard drop a block
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock() ;
        }
    }
}
