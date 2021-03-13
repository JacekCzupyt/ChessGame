using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using ChessGame.GameLogic;

namespace ChessGame
{
    /// <summary>
    /// Interaction logic for ChessPage.xaml
    /// </summary>
    public partial class ChessPage : Page
    {
        //List<List<VisualChessPiece>> BoardState = new List<List<VisualChessPiece>>();
        List<List<VisualChessTile>> TileMap = new List<List<VisualChessTile>>();
        VisualChessTile SelectedTile = null;
        ChessBoard GameBoard = ChessBoard.DefaultChessSetup();
        Dictionary<ChessPiece, VisualChessPiece> PieceDictionary = new Dictionary<ChessPiece, VisualChessPiece>();
        Dictionary<VisualChessTile, List<ChessMove>> MoveDictionary = new Dictionary<VisualChessTile, List<ChessMove>>();

        event EventHandler<ChessMoveEventArgs> CommitMove;

        ChessBot1 ChessBot = new ChessBot1();

        public ChessPage()
        {
            InitializeComponent();
        }

        private void test1(object sender, ChessMoveEventArgs e)
        { }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateBoard();
            UpdateBoard(this, null);
            GameBoard.PropertyChanged += UpdateBoard;

            //CommitMove += test1;


            CommitMove += GameBoard.AtachPlayer(new Action(() => { })/*do nothing, to be replaceed*/, ChessColor.White);
            //GameBoard.AtachPlayer(new Action(() => { })/*do nothing, to be replaceed*/, ChessColor.Black);

            ChessBot.Board = GameBoard;
            ChessBot.CommitMove += GameBoard.AtachPlayer(ChessBot.MoveRequest, ChessColor.Black);

            GameBoard.Begin();

            //temp

            //VisualChessPiece p = new VisualChessPiece(new Uri("pack://application:,,,/textures/Chess_bdt60.png"));
            //ChessGrid.Children.Add(p);
            //p.Tile = TileMap[0][0];

        }

        private void CreateBoard()
        {
            for(int i=0;i<8;i++)
            {
                //BoardState.Add(new List<VisualChessPiece>());
                TileMap.Add(new List<VisualChessTile>());
                for(int j=0;j<8;j++)
                {
                    //BoardState[i].Add(null);
                    VisualChessTile x = new VisualChessTile(this, i, j);
                    Grid.SetColumn(x, i);
                    Grid.SetRow(x, 7-j);
                    ChessGrid.Children.Add(x);
                    TileMap[i].Add(x);
                }
            }
        }

        //temp
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is VisualChessTile))
                throw new ArgumentException();
            var tile = sender as VisualChessTile;
            if (SelectedTile == null && tile.Piece != null)
            {
                SelectTile(tile);
            }
            else if(SelectedTile!=null && MoveDictionary.ContainsKey(tile))
            {
                ExecuteMove(tile);
            }
            else if(SelectedTile!=null)
            {
                SelectedTile.Deselect();
                SelectedTile = null;
                foreach (var t in MoveDictionary.Keys)
                    t.Deselect();
                MoveDictionary.Clear();
            }
        }

        private void SelectTile(VisualChessTile tile)
        {
            SelectedTile = tile;
            SelectedTile.Select();
            if (GameBoard[(tile.x, tile.y)] == null)
                throw new ArgumentException();
            foreach(var Move in GameBoard[(tile.x, tile.y)].LegalMoves)
            {
                var moveTile = TileMap[Move.Dest.Item1][Move.Dest.Item2];
                moveTile.Highlight();
                if(!MoveDictionary.ContainsKey(moveTile))
                {
                    MoveDictionary[moveTile] = new List<ChessMove>() { Move };
                }
                else
                {
                    MoveDictionary[moveTile].Add(Move);
                }
            }
        }

        private void ExecuteMove(VisualChessTile tile)
        {
            if(MoveDictionary[tile].Count==1)
            {
                var m = MoveDictionary[tile].First();
                CommitMove(this, new ChessMoveEventArgs() { Move = m });
            }
            else
            {
                PromotionWindow PromotionDialog = new PromotionWindow();
                if(PromotionDialog.ShowDialog()==true)
                {
                    SpecialState s = SpecialState.None;
                    if(PromotionDialog.PressedButton==PromotionDialog.QueenButton)
                    {
                        s = SpecialState.PawnMorphQueen;
                    }
                    if (PromotionDialog.PressedButton == PromotionDialog.RookButton)
                    {
                        s = SpecialState.PawnMorphRook;
                    }
                    if (PromotionDialog.PressedButton == PromotionDialog.BishopButton)
                    {
                        s = SpecialState.PawnMorphBishop;
                    }
                    if (PromotionDialog.PressedButton == PromotionDialog.KnightButton)
                    {
                        s = SpecialState.PawnMorphKnight;
                    }

                    var m = MoveDictionary[tile].Find(item => item.State[0] == s);
                    CommitMove(this, new ChessMoveEventArgs() { Move = m });
                }
                else
                {
                    return;
                }
            }
            SelectedTile.Deselect();
            SelectedTile = null;
            foreach (var t in MoveDictionary.Keys)
                t.Deselect();
            MoveDictionary.Clear();
        }

        private void UpdateBoard(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            foreach(ChessPiece piece in GameBoard.PieceList)
            {
                VisualChessPiece visualPiece;
                VisualChessTile tile = piece.InPlay ? TileMap[piece.Position.x][piece.Position.y] : null;
                if(PieceDictionary.TryGetValue(piece, out visualPiece))
                {
                    visualPiece.Tile = tile;
                }
                else
                {
                    //create new visual piece
                    PieceDictionary.Add(piece, new VisualChessPiece(ChessGrid, piece, tile));
                }
            }
        }
    }
}
