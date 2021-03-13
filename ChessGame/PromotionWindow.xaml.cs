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
using System.Windows.Shapes;

namespace ChessGame
{
    /// <summary>
    /// Interaction logic for PromotionWindow.xaml
    /// </summary>
    public partial class PromotionWindow : Window
    {
        public Button PressedButton = null;

        public PromotionWindow()
        {
            InitializeComponent();
        }

        private void PromotionButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
                throw new InvalidOperationException();
            PressedButton = sender as Button;
            this.DialogResult = true;
        }
    }
}
