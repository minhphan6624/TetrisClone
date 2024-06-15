using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
        };

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;

        private GameState gameState = new GameState(); 

        //Constructor
        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns]; 
            int cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10); //2 hidden rows on top
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawGrid(GameGrid grid) //Draw the game grid
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c]; //Get the id of the tile in that position

                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[id]; //Set the image of the tile using the id
                }
            }
        }
        private void DrawBlock(Block block)                     //Draw a block
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)       //Draw the next block
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];            //Set the source of the NextImage image to the image of that block 
        }

        private void DrawHeldBlock(Block heldBlock)             //Draw the block that is currently on hold
        {
            if (heldBlock == null)
            {
                HoldImage.Source = blockImages[0];              
            }
            else
            {
                HoldImage.Source = blockImages[heldBlock.Id];
            }
        }

        private void DrawGhostBlock(Block block)                //Draw the ghost block
        {
            int dropDistance = gameState.BlockDropDistance();

            //The cells where the ghost block = current cell positions + drop distance
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;                   //Set the opacity of the ghost block
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];    //Set the image of the ghost block
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
            Score.Text = $"Score: {gameState.Score}";
        }

        private async Task GameLoop()
        {
            Draw(gameState);                            //Draw the current game state

            while (!gameState.GameOver)                 //Loop until the game is over
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score/10 * delayDecrease)); //increase the dropping frequency
                await Task.Delay(delay);                  
                gameState.MoveBlockDown();              //Move the block down
                Draw(gameState);                        //Redraw
            }

            GameOverMenu.Visibility = Visibility.Visible;       //Show the Game Over menu when the game is over       
            FinalScoreText.Text = $"Score: {gameState.Score}"; //Set the final score on the Game Over menu
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver) //If the game is over, pressing keys won't do anything
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();          //Left arrow for moving left
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();         //Right arrow for moving right
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();          //Down arrow for moving down
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();          //Up arrow for rotating clockwise
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW();         //Z key for rotating counter-clockwise
                    break;
                case Key.C:
                    gameState.HoldBlock();              //C key for holding the current block
                    break;
               case Key.Space:
                    gameState.DropBlock();              //Spacebar for hard dropping the current block
                    break;
                default:
                    return;
            }
            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e) //When the "Play Again" button is clicked
        {
            gameState = new GameState();                        //Refresh the game state
            GameOverMenu.Visibility = Visibility.Hidden;        //Hide the game over menu
            await GameLoop();                                   //Start the game loop
        }
    }
}