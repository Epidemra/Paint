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

namespace Paint
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<UIElement> deletedFigures = new List<UIElement>();
        byte tag = 0;
        bool clicked = false;
        bool drawFirst = true;
        Point a, b;
        Button currentFigure = null;
        FigureList shapes = new FigureList();
        int thickness = 1;

        public MainWindow()
        {
            InitializeComponent();

            currentFigure = Line;
            currentFigure.Background = Brushes.DarkCyan;

            inkCanvas.Background = Brushes.White;
        }

        private void GetFigureType(object sender, RoutedEventArgs e)
        {
            tag = Convert.ToByte(((Button)sender).Uid);
            var backgrounColor = ((Button)sender).Background;

            currentFigure.Background = backgrounColor;

            currentFigure = (Button)sender;
            ((Button)sender).Background = Brushes.DarkCyan;
        }

        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Children.Clear();
            deletedFigures.Clear();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            thickness = (int)e.NewValue;
        }

        private void undo(object sender, RoutedEventArgs e)
        {
            if (inkCanvas.Children.Count != 0)
            {
                deletedFigures.Add(inkCanvas.Children[inkCanvas.Children.Count - 1]);
                inkCanvas.Children.RemoveAt(inkCanvas.Children.Count - 1);
            }
        }

        private void redo(object sender, RoutedEventArgs e)
        {
            if (deletedFigures.Count != 0)
            {
                inkCanvas.Children.Add(deletedFigures[deletedFigures.Count - 1]);
                deletedFigures.RemoveAt(deletedFigures.Count - 1);
            }
        }

        private void ChangingObjects()
        {
            shapes.upDate();
            foreach (Figure fig in shapes.figureList.Values)
            {
                fig.firstPoint = a;
                fig.secondPoint = b;
                fig.thickness = thickness;
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            clicked = true;
            drawFirst = true;
            a = e.GetPosition(inkCanvas);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (clicked && (e.LeftButton == MouseButtonState.Pressed))
            {
                b = e.GetPosition(inkCanvas);
                ChangingObjects();
                if (!drawFirst)
                    inkCanvas.Children.RemoveAt(inkCanvas.Children.Count - 1);
                drawFirst = false;
                switchHighlighter.Content = tag.ToString();
                shapes.figureList[tag].Draw(inkCanvas);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            clicked = false;
            drawFirst = true;
        }

    }
}
