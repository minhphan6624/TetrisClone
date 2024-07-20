using Microsoft.Windows.Themes;
using System.Collections.Generic;

namespace Tetris
{
    public abstract class Block
    {
        //Attributes
        private Position _offset; //The current offset of the block
        private int _rotationState; //The current rotation state of the block
        
        //Properties
        protected abstract Position[][] Tiles { get; } //Contains the tile positions in each rotation state
        protected abstract Position StartOffset { get; } //Where the block spawns 
        public abstract int Id { get; } //The block Id

        //Constructor
        public Block()
        {
            _offset = new Position(StartOffset.Row, StartOffset.Column);
        }

        //Methods
        public IEnumerable<Position> TilePositions() 
        {
            foreach (Position p in Tiles[_rotationState])
            {
                yield return new Position(p.Row + _offset.Row, p.Column + _offset.Column);
            }
        }

        public void RotateCW() //Rotate the block clockwise 
        {
            _rotationState = (_rotationState + 1) % Tiles.Length;       //Switch to the next rotation state
        }

        public void RotateCCW() //Rotate the block counter-clockwise ()
        {
            if (_rotationState == 0)
            {
                _rotationState = Tiles.Length - 1; 
            }
            else
            {
                _rotationState--;
            }
        }

        public void Move(int rows, int columns)     //Move the block by a number of rows and columns
        {
            _offset.Row += rows;
            _offset.Column += columns;
        }

        public void Reset()                         //Reset the block
        {
            _rotationState = 0;
            _offset.Row = StartOffset.Row;
            _offset.Column = StartOffset.Column;
        }
    }
}