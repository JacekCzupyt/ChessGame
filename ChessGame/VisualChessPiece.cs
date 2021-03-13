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
using ChessGame.GameLogic;
using ChessGame.GameLogic.ChessPieces;


namespace ChessGame
{
    partial class ChessPage : Page
    {
        class VisualChessPiece : Image
        {
            static Dictionary<Type, char> AddressDictionary = new Dictionary<Type, char>()
            {
                {typeof(Pawn), 'p'},
                {typeof(Rook), 'r'},
                {typeof(Knight), 'n'},
                {typeof(Bishop), 'b' },
                {typeof(Queen), 'q' },
                {typeof(King), 'k' }
            };

            VisualChessTile tile;

            public VisualChessPiece(Grid ChessGrid, Uri s, VisualChessTile t = null)
            {
                IsHitTestVisible = false;
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = s;
                logo.EndInit();
                Source = logo;
                Panel.SetZIndex(this, 1);
                Tile = t;
                ChessGrid.Children.Add(this);
            }

            private Uri UriFromChessPiece(ChessPiece Piece)
            {
                string s = "pack://application:,,,/textures/Chess_";
                s += AddressDictionary[Piece.GetType()];
                if (Piece.Color == GameLogic.ChessColor.White)
                    s += 'l';
                else
                    s += 'd';
                s += "t60.png";
                return new Uri(s);
            }

            public VisualChessPiece(Grid ChessGrid, ChessPiece Piece, VisualChessTile t = null)
            {
                IsHitTestVisible = false;
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = UriFromChessPiece(Piece);
                logo.EndInit();
                Source = logo;
                Panel.SetZIndex(this, 1);
                Tile = t;
                ChessGrid.Children.Add(this);
            }

            public VisualChessTile Tile
            {
                get { return tile; }
                set
                {
                    if(tile!=value)
                    {
                        if(tile!=null)
                        {
                            tile.Piece = null;
                        }
                        if (value != null)
                        {
                            if (value.Piece != null)
                                value.Piece.Tile = null;
                            tile = value;
                            tile.Piece = this;
                            Grid.SetColumn(this, tile.x);
                            Grid.SetRow(this, 7 - tile.y);
                            Visibility = Visibility.Visible;
                        }
                        if(value == null)
                        {
                            Visibility = Visibility.Collapsed;
                            tile = null;
                        }
                            
                    }
                }
            }
        }
    }
    
}
