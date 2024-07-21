using System;

namespace Tetris
{
    public class BlockQueue
    {
        //Attribute
        private readonly Block[] blocks = new Block[]               //The list of all the blocks in the game
       {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
       };

        private Block _nextBlock;

        private readonly Random _random = new Random();

        //Property
        public Block NextBlock
        {
            get => _nextBlock;
            set => _nextBlock = value;
        }

        //Constructor
        public BlockQueue()
        {
            _nextBlock = RandomBlock();
        }

        //Methods
        private Block RandomBlock()                         //Return a random block
        {
            return blocks[_random.Next(blocks.Length)];
        }

         //Returns the next block and update the property
        public Block GetAndUpdate()                        
        {
            Block block = NextBlock;

            do
            {
                NextBlock = RandomBlock();
            }
            while (block.Id == NextBlock.Id);               //Avoid the next block being the same as the current one

            return block;
        }
    }
}
