namespace Tetris
{
    public class Position
    {
        //Attributes
        private int _row;
        private int _column;

        //Properties
        public int Row
        {
            get => _row;
            set => _row = value;
        }
        public int Column
        {
            get => _column;
            set => _column = value;
        }
        //Constructors
        public Position (int row, int column)
        {
            Row = row;
            Column = column;  
        }
    }
}
