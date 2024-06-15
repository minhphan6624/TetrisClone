namespace Tetris
{
    public class GameGrid
    {
        //Attributes
        private readonly int[,] _grid;      //The acutal game grid
        private int _rows;                  //The number of rows
        private int _columns;               //The number of columns

        //Properties
        public int Rows { 
            get => _rows;
            set => _rows = value;
        }
        public int Columns { 
            get => _columns;
            set => _columns = value;
        }

        public int this[int r, int c]
        {
            get => _grid[r, c];
            set => _grid[r, c] = value;
        }

        //Constructor
        public GameGrid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            _grid = new int[rows, columns];
        }

        //Methods
        public bool IsInside(int r, int c)                  //Check if a cell is inside the Game _grid
        {
            return r >= 0 && r < Rows && c >= 0 && c < Columns;
        }

        public bool IsEmpty(int r, int c)                   //Check if a cell is empty
        {
            return IsInside(r, c) && _grid[r, c] == 0;
        }

        public bool IsRowFull(int r)                        //Check if an entire row is full 
        {
            for (int c = 0; c < Columns; c++)
            {
                if (_grid[r, c] == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsRowEmpty(int r)                       // Check if an entire row is empty
        {
            for (int c = 0; c < Columns; c++)
            {
                if (_grid[r, c] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void ClearRow(int r)                        //Clear a row
        {
            for (int c = 0; c < Columns; c++)
            {
                _grid[r, c] = 0;
            }
        }

        private void MoveRowDown(int r, int numRows)        //Move a row down by a number of rows
        {
            for (int c = 0; c < Columns; c++)
            {
                _grid[r + numRows, c] = _grid[r, c]; 
                _grid[r, c] = 0;
            }
        }

        public int ClearFullRows()                          //Clear multiple rows
        {
            int cleared = 0;                                //number of cleared rows

            for (int r = Rows - 1; r >= 0; r--)
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);                            //Clear row if that row is full
                    cleared++;                              //Increase the number of cleared row
                }
                else if (cleared > 0)
                {
                    MoveRowDown(r, cleared);                //Move row down by the number of cleared rows
                }
            }

            return cleared;                                 //return the number of row cleared
        }
    }
}