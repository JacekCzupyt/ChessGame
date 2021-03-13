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

namespace ChessGame
{
    partial class ChessPage : Page
    {
        private class VisualChessTile : Button
        {
            public VisualChessTile(ChessPage page, int _x, int _y)
            {
                x = _x; y = _y;
                if ((x + y) % 2 == 0)
                    DefaultColor = Brushes.DarkGray;
                else
                    DefaultColor = Brushes.GhostWhite;

                Background = DefaultColor;
                BorderBrush = Brushes.Black;
                BorderThickness = new Thickness(1);
                Page = page;
                this.Click += Page.Tile_Click;
            }
            public int x;
            public int y;
            Brush DefaultColor;
            
            readonly ChessPage Page;

            public VisualChessPiece Piece = null;

            public void Select()
            {
                BorderBrush = System.Windows.Media.Brushes.Blue;
                BorderThickness = new Thickness(5);
            }

            public void Highlight()
            {
                BorderBrush = System.Windows.Media.Brushes.Yellow;
                BorderThickness = new Thickness(5);
            }

            public void Deselect()
            {
                BorderBrush = Brushes.Black;
                BorderThickness = new Thickness(1);
            }
        }
    }
}
