﻿using System;
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
using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Interfaces;


	/*<runtime>
		<assemblyBinding xmlns = "urn:schemas-microsoft-com:asm.v1" >

            < probing privatePath="DLL" />
		</assemblyBinding>
	</runtime>*/

namespace Paint
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    //enum  dest { UpRight, UpLeft, DownLeft, DownRight};
    //public enum resize { firstX, secondX, firstY, secondY};


    public partial class MainWindow : Window
    {
        //Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        Dictionary<string, Type> types = new Dictionary<string, Type>();
        Dictionary<string, ICreator> creators = new Dictionary<string, ICreator>();
        Stack<UIElement> deletedFigures = new Stack<UIElement>();
        byte tag = 0;
        bool isDrawing = false;
        bool isMoving = false;
        bool isResizing = false;
        bool moveFirst = false;
        bool isCanceled = false;
        bool isSecondHoriz, isSecondVert, isFirstHoriz, isFirstVert;
        bool isRedo = false;
        bool drawFirst = true;
        Point a, b, startResize;
        Button currentFigure = null;
        //FigureList shapes = new FigureList();
        int thickness = 1;
        List<IFigure> printedFigures = new List<IFigure>();
        Stack<IFigure> removedFigures = new Stack<IFigure>();
        Point x, y;
        //direction checkedDir;
        UIElement chosenFigUI = null;
        IFigure chosenFig = null;
        IFigure current;
        /*Creator[] creators = new Creator[] { EllipseCreator.getInstance(), CircleCreator.getInstance(),
                                            equalSideTriangleCreator.getInstance()};*/
        public static Shape frame = null;
        dynamic fig = null;
        string currentType = null;

        public MainWindow()
        {
            InitializeComponent();
            Focus();
            KeyDown += MainWindow_KeyDown;

            inkCanvas.Background = Brushes.White;
            int i = 0;

            string dirName = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            foreach (string file in Directory.EnumerateFiles(dirName, "*.dll"))
            {
                ICreator creator = null;
                MessageBox.Show(file);
                Assembly asm = Assembly.LoadFrom(file);
                foreach (Type t in asm.GetExportedTypes())
                {
                    if (t.IsClass && typeof(ICreator).IsAssignableFrom(t))
                    {
                        creator = (ICreator)Activator.CreateInstance(t);
                        break;
                    }
                }

                foreach (Type t in asm.GetExportedTypes())
                {
                    if (t.IsClass && typeof(IFigure).IsAssignableFrom(t))
                    {
                        if (creator != null)
                        {
                            types.Add(t.ToString(), t);
                            creators.Add(t.ToString(), creator);
                            var cmbItem = new ComboBoxItem();
                            cmbItem.Content = t.ToString();
                            if (i == 0)
                            {
                                currentType = t.ToString();
                                cmbItem.IsSelected = true;
                                i++;
                            }
                            shapeList.Items.Add(cmbItem);
                        }
                        break;
                    }
                }

                /*Type[] typess = asm.GetTypes();
                Type type = typess[1];
                MethodInfo getInstance = typess[0].GetMethod("getInstance");
                //creators.Add(type.ToString(), (Creator)getInstance.Invoke(null, null));
                var fig = (Figure)Activator.CreateInstance(type);
                if (!(fig is Figure))
                    continue;
                types.Add(type.ToString(), type);
                //var cmbItem = new ComboBoxItem();
                cmbItem.Content = fig.GetType();//.Split('.')[1];
                if (i == 0)
                {
                    currentType = type.ToString();
                    cmbItem.IsSelected = true;
                    i++;
                }
                shapeList.Items.Add(cmbItem);*/

               /* foreach (MethodInfo meth in type.GetMethods())
                {
                    methods.Add(type.ToString() + '-' + meth.Name, meth);
                } */
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                if (inkCanvas.Children.IndexOf(frame) != -1)
                    inkCanvas.Children.Remove(frame);
                deletedFigures.Push(chosenFigUI);
                removedFigures.Push(chosenFig);
                inkCanvas.Children.Remove(chosenFigUI);
                printedFigures.Remove(chosenFig);
                chosenFig = null;
                chosenFigUI = null;
                isCanceled = true;
            }
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
            printedFigures.Clear();
            removedFigures.Clear();
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
            if (inkCanvas.Children.IndexOf(frame) != -1)
                inkCanvas.Children.Remove(frame);
            if (inkCanvas.Children.Count != 0)
            {
                deletedFigures.Push(inkCanvas.Children[inkCanvas.Children.Count - 1]);
                removedFigures.Push(printedFigures[printedFigures.Count - 1]);
                inkCanvas.Children.RemoveAt(inkCanvas.Children.Count - 1);
                printedFigures.RemoveAt(printedFigures.Count - 1);
                isCanceled = true;
                switchHighlighter.Content = deletedFigures.Count;
            }
        }

        private void redo(object sender, RoutedEventArgs e)
        {
            if (deletedFigures.Count != 0 && isCanceled)
            {
                inkCanvas.Children.Add(deletedFigures.Pop());
                printedFigures.Add(removedFigures.Pop());
                isRedo = true;
                //printedFigures[printedFigures.Count - 1].Draw(inkCanvas);
                /*methods[printedFigures[printedFigures.Count - 1].GetType().ToString() + '-' + "Draw"].Invoke(printedFigures[printedFigures.Count - 1], new Object[] { inkCanvas });
                */
            }
        }

        /*private void ChangingObjects()
        {
            shapes.upDate();
            foreach (Figure fig in shapes.figureList.Values)
            {
                fig.firstPoint = a;
                fig.secondPoint = b;
                fig.thickness = thickness;
            }
        }*/

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            drawFirst = true;
            a = e.GetPosition(inkCanvas);
            //var canv = (Canvas)sender;
            //
            //current = creators[tag].Create();
            //
            //MessageBox.Show(currentType);
            //current = (Figure)Activator.CreateInstance(types[currentType]);
            current = creators[currentType].Create(new Point(0,0), new Point(0,0), thickness);
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            System.Windows.Shapes.Shape rect = (System.Windows.Shapes.Shape)(sender);
            chosenFigUI = rect;
           
            for (int i = 0; i < printedFigures.Count; i++)
                if (printedFigures[i].hash == chosenFigUI.GetHashCode())
                {
                    chosenFig = printedFigures[i];
                    x = chosenFig.firstPoint;
                    y = chosenFig.secondPoint;
                    break;
                }

            //fig = chosenFig;
            if(chosenFig is ISelectable)
            {

                isDrawing = false;
                moveFirst = true;
                isMoving = chosenFig is IMovable;

                if (frame != null && inkCanvas.Children.IndexOf(frame) != -1)
                    inkCanvas.Children.Remove(frame);
                inkCanvas.Children.Remove(chosenFigUI);
                inkCanvas.Children.Add(chosenFigUI);
                printedFigures.Remove(chosenFig);
                printedFigures.Add(chosenFig);

                var selectable = (ISelectable)chosenFig;
                selectable.Select(inkCanvas, ref frame);
                //fig.Select(inkCanvas, ref frame);
                //methods[fig.GetType().ToString() + '-' + "Select"].Invoke(fig, new Object[] { inkCanvas });
                //select.Invoke(fig, new Object[] { inkCanvas});
                //fig.Select(inkCanvas);
                frame.Stroke = Brushes.Blue;
                frame.Cursor = Cursors.SizeAll;
                frame.MouseDown += Frame_MouseDown;
                frame.MouseUp += Frame_MouseUp;
            }
            else
            {
                chosenFig = null;
                chosenFigUI = null;
                fig = null;
            }
        }

        public void DrawFrame(IFigure figure)
        {
            frame = new System.Windows.Shapes.Rectangle();
            frame.Width = figure.getWidth() + 10;
            frame.Height = figure.getHeight() + 10;
            frame.Stroke = Brushes.Blue;
            frame.Cursor = Cursors.SizeAll;
            frame.MouseDown += Frame_MouseDown;
            frame.MouseUp += Frame_MouseUp;

            inkCanvas.Children.Add(frame);

            if (figure.firstPoint.X < figure.secondPoint.X && figure.firstPoint.Y <= figure.secondPoint.Y)
            {
                Canvas.SetLeft(frame, figure.firstPoint.X - 5);
                Canvas.SetTop(frame, figure.firstPoint.Y - 5);
            }
            else if (figure.firstPoint.X <= figure.secondPoint.X && figure.firstPoint.Y > figure.secondPoint.Y)
            {
                Canvas.SetLeft(frame, figure.firstPoint.X - 5);
                Canvas.SetBottom(frame, inkCanvas.ActualHeight - figure.firstPoint.Y - 5);
            }
            else if (figure.firstPoint.X > figure.secondPoint.X && figure.firstPoint.Y >= figure.secondPoint.Y)
            {
                Canvas.SetRight(frame, inkCanvas.ActualWidth - figure.firstPoint.X - 5);
                Canvas.SetBottom(frame, inkCanvas.ActualHeight - figure.firstPoint.Y - 5);
            }
            else if (figure.firstPoint.X >= figure.secondPoint.X && figure.firstPoint.Y < figure.secondPoint.Y)
            {
                Canvas.SetTop(frame, figure.firstPoint.Y - 5);
                Canvas.SetRight(frame, inkCanvas.ActualWidth - figure.firstPoint.X - 5);
            }
        }

        private void Frame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isResizing = false;
        }

        private void Frame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if(fig != null && fig is IResizable)
            if (chosenFig != null && chosenFig is IResizable)
            {
                isResizing = true;
                moveFirst = true;
                startResize = e.GetPosition(inkCanvas);
                Point point = e.GetPosition(inkCanvas);

                isFirstHoriz = (Math.Abs(point.X - chosenFig.firstPoint.X) < Math.Abs(point.X - chosenFig.secondPoint.X)) &&
                               (point.Y > Math.Min(chosenFig.firstPoint.Y, chosenFig.secondPoint.Y) && point.Y < Math.Max(chosenFig.firstPoint.Y, chosenFig.secondPoint.Y));
                isSecondHoriz = (Math.Abs(point.X - chosenFig.firstPoint.X) > Math.Abs(point.X - chosenFig.secondPoint.X)) &&
                                (point.Y > Math.Min(chosenFig.firstPoint.Y, chosenFig.secondPoint.Y) && point.Y < Math.Max(chosenFig.firstPoint.Y, chosenFig.secondPoint.Y));
                isFirstVert = (Math.Abs(point.Y - chosenFig.firstPoint.Y) < Math.Abs(point.Y - chosenFig.secondPoint.Y)) &&
                              (point.X > Math.Min(chosenFig.firstPoint.X, chosenFig.secondPoint.X) && point.X < Math.Max(chosenFig.firstPoint.X, chosenFig.secondPoint.X)); ;
                isSecondVert = (Math.Abs(point.Y - chosenFig.secondPoint.Y) < Math.Abs(point.Y - chosenFig.firstPoint.Y)) &&
                               (point.X > Math.Min(chosenFig.firstPoint.X, chosenFig.secondPoint.X) && point.X < Math.Max(chosenFig.firstPoint.X, chosenFig.secondPoint.X));

            }

            //inkCanvas.Children.Remove(chosenFigUI);
        }

        private void shapeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            currentType = selectedItem.Content.ToString();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing && (e.LeftButton == MouseButtonState.Pressed))
            {
                var resizable = (IResizable)chosenFig;
                if (moveFirst)
                    inkCanvas.Children.Remove(chosenFigUI);
                else
                    inkCanvas.Children.RemoveAt(inkCanvas.Children.Count - 2);
                //chosenFigUI = null;

                moveFirst = false;
                b = e.GetPosition(inkCanvas);
                if (isFirstHoriz)
                {
                    //methods[currentType + '-' + "Resize"].Invoke(fig, new Object[] { e.GetPosition(inkCanvas).X - startResize.X, resize.firstX });
                    //fig.Resize(e.GetPosition(inkCanvas).X - startResize.X, resize.firstX);
                    resizable.Resize(e.GetPosition(inkCanvas).X - startResize.X, resize.firstX);
                }
                else
                if (isSecondHoriz)
                {
                    resizable.Resize(e.GetPosition(inkCanvas).X - startResize.X, resize.secondX);
                    //fig.Resize(e.GetPosition(inkCanvas).X - startResize.X, resize.secondX);
                    //methods[currentType + '-' + "Resize"].Invoke(fig, new Object[] { e.GetPosition(inkCanvas).X - startResize.X, resize.secondX });
                }
                else
                if (isFirstVert)
                {
                    resizable.Resize(e.GetPosition(inkCanvas).Y - startResize.Y, resize.firstY);
                    //fig.Resize(e.GetPosition(inkCanvas).Y - startResize.Y, resize.firstY);
                   // methods[currentType + '-' + "Resize"].Invoke(fig, new Object[] { e.GetPosition(inkCanvas).Y - startResize.Y, resize.firstY });
                }                    
                else
                if (isSecondVert)
                {
                    resizable.Resize(e.GetPosition(inkCanvas).Y - startResize.Y, resize.secondY);
                   // fig.Resize(e.GetPosition(inkCanvas).Y - startResize.Y, resize.secondY);
                   // methods[currentType + '-' + "Resize"].Invoke(fig, new Object[] { e.GetPosition(inkCanvas).Y - startResize.Y, resize.secondY });
                }

                //methods[currentType + '-' + "Draw"].Invoke(fig, new Object[] { inkCanvas});
                //fig.Draw(inkCanvas);
                chosenFig.Draw(inkCanvas);
                inkCanvas.Children[inkCanvas.Children.Count - 1].MouseDown += MainWindow_MouseDown;
                inkCanvas.Children[inkCanvas.Children.Count - 1].MouseUp += MainWindow_MouseUp;
                ((System.Windows.Shapes.Shape)inkCanvas.Children[inkCanvas.Children.Count - 1]).Cursor = Cursors.Cross;
                chosenFigUI = inkCanvas.Children[inkCanvas.Children.Count - 1];
                inkCanvas.Children.Remove(frame);
                // DrawFrame(chosenFig);
                //methods[currentType + '-' + "Select"].Invoke(fig, new Object[] { inkCanvas});
                //fig.Select(inkCanvas, ref frame);
                var selectable = (ISelectable)chosenFig;
                selectable.Select(inkCanvas, ref frame);
                frame.Stroke = Brushes.Blue;
                frame.Cursor = Cursors.SizeAll;
                frame.MouseDown += Frame_MouseDown;
                frame.MouseUp += Frame_MouseUp;
                startResize = e.GetPosition(inkCanvas);
                return;
            }
            else
            if (chosenFigUI != null && chosenFig != null && (e.LeftButton == MouseButtonState.Pressed) && isMoving)
            {
                var movable = (IMovable)chosenFig;
                if (moveFirst)
                    inkCanvas.Children.Remove(chosenFigUI);
                else
                    inkCanvas.Children.RemoveAt(inkCanvas.Children.Count - 2);
                    
                moveFirst = false;
                ///switchHighlighter.Content = isCanceled.ToString();
                b = e.GetPosition(inkCanvas);
                //methods[fig.GetType().ToString() + '-' + "Move"].Invoke(fig, new Object[] { inkCanvas, b.X - a.X, b.Y - a.Y, x, y });
                //fig.Move(inkCanvas, b.X - a.X, b.Y - a.Y, x, y);
                movable.Move(inkCanvas, b.X - a.X, b.Y - a.Y, x, y);
                chosenFig.Draw(inkCanvas);
                //methods[chosenFig.GetType().ToString() + '-' + "Draw"].Invoke(chosenFig, new Object[] { inkCanvas });
                chosenFigUI = inkCanvas.Children[inkCanvas.Children.Count - 1];
                switchHighlighter.Content = chosenFig.GetType();
                inkCanvas.Children[inkCanvas.Children.Count - 1].MouseDown += MainWindow_MouseDown;
                inkCanvas.Children[inkCanvas.Children.Count - 1].MouseUp += MainWindow_MouseUp;
                ((System.Windows.Shapes.Shape)inkCanvas.Children[inkCanvas.Children.Count - 1]).Cursor = Cursors.Cross;
                inkCanvas.Children.Remove(frame);
                //DrawFrame(chosenFig);
                //fig.Select(inkCanvas, ref frame);
                var selectable = (ISelectable)chosenFig;
                selectable.Select(inkCanvas, ref frame);
                //methods[fig.GetType().ToString() + '-' + "Select"].Invoke(fig, new Object[] { inkCanvas });
                frame.Stroke = Brushes.Blue;
                frame.Cursor = Cursors.SizeAll;
                frame.MouseDown += Frame_MouseDown;
                frame.MouseUp += Frame_MouseUp;
                switchHighlighter.Content = printedFigures.Count;
                return;
            }
            else
            if (isDrawing && (e.LeftButton == MouseButtonState.Pressed) && !isResizing && !isMoving)
            {
                b = e.GetPosition(inkCanvas);
                /*current = creators[tag].Create();
                current.firstPoint = a;
                current.secondPoint = b;
                current.thickness = thickness;*/
                //current = (Figure)Activator.CreateInstance(types[currentType], new Object[] { a, b, thickness});
                current = creators[currentType].Create(a, b, thickness);
               // ChangingObjects();
                if (!drawFirst)
                {
                    inkCanvas.Children.RemoveAt(inkCanvas.Children.Count - 1);
                    printedFigures.RemoveAt(printedFigures.Count - 1);
                }                    
                drawFirst = false;
                switchHighlighter.Content = tag.ToString();
                //printedFigures.Add(shapes.figureList[tag]);
                //shapes.figureList[tag].Draw(inkCanvas);
                printedFigures.Add(current);
                //methods[currentType + '-' + "Draw"].Invoke(current, new Object[] { inkCanvas });
                current.Draw(inkCanvas);
                inkCanvas.Children[inkCanvas.Children.Count - 1].MouseDown += MainWindow_MouseDown;
                inkCanvas.Children[inkCanvas.Children.Count - 1].MouseUp += MainWindow_MouseUp;
                ((System.Windows.Shapes.Shape)inkCanvas.Children[inkCanvas.Children.Count - 1]).Cursor = Cursors.Cross;

                isCanceled = false;
                //isRedo = false;
                deletedFigures.Clear();
                removedFigures.Clear();
                return;
            }
        }

        private void MainWindow_MouseEnter(object sender, MouseEventArgs e)
        {

            /*if (Math.Sqrt(Math.Pow((e.GetPosition(inkCanvas).X - chosenFig.firstPoint.X), 2) + Math.Pow((e.GetPosition(inkCanvas).Y - chosenFig.firstPoint.Y), 2)) <= 10)
                Cursor = Cursors.SizeNESW;*/

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "(*.dat)|*.dat";
            if(saveFileDialog.ShowDialog() == true)
            {                
                using (var fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    BinaryWriter binaryWriter = new BinaryWriter(fs);
                    var formatter = new BinaryFormatter();
                    foreach (IFigure fig in printedFigures)
                    {
                        formatter.Serialize(fs, fig);
                        binaryWriter.Write((byte)'$');
                    }
                    binaryWriter.Close();
                }
            }         
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "(*.dat)|*.dat";
            if (openFileDialog.ShowDialog() == true)
            {
                using (var fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    inkCanvas.Children.Clear();
                    printedFigures.Clear();
                    removedFigures.Clear();
                    deletedFigures.Clear();
                    BinaryReader vd = new BinaryReader(fs);
                    var formatter = new BinaryFormatter();
                    //long lastPos = 0;
                    while (fs.Position < fs.Length)
                    {
                        long backUp = fs.Position;
                        while (fs.Position < fs.Length && vd.ReadByte() != (byte)'$') { }
                        long delimPos = fs.Position;
                        fs.Position = backUp;
                        try
                        {
                            var figure = (IFigure)formatter.Deserialize(fs);
                            if (types.Values.ToList().IndexOf(figure.GetType()) != -1)
                            {
                                figure.Draw(inkCanvas);
                                //methods[figure.GetType().ToString() + '-' + "Draw"].Invoke(figure, new Object[] { inkCanvas });
                                //select.Invoke(figure, new Object[] { inkCanvas});
                                //figure.Draw(inkCanvas);
                                printedFigures.Add(figure);
                                inkCanvas.Children[inkCanvas.Children.Count - 1].MouseDown += MainWindow_MouseDown;
                                inkCanvas.Children[inkCanvas.Children.Count - 1].MouseUp += MainWindow_MouseUp;
                                ((System.Windows.Shapes.Shape)inkCanvas.Children[inkCanvas.Children.Count - 1]).Cursor = Cursors.Cross;
                            }
                            if (fs.Position < fs.Length)
                                vd.ReadByte();
                            //lastPos = fs.Position;
                        }
                        catch (Exception)
                        {
                            /*if (fs.Position - lastPos > 425)
                               while (fs.Position >= 0 && vd.ReadByte() != (byte)'$') { fs.Position -= 2; }
                           else
                               while (fs.Position < fs.Length && vd.ReadByte() != (byte)'$') { }*/
                            fs.Position = delimPos;
                            /*if (fs.Position > delimPos)
                            {
                                //fs.Position--;
                                while (fs.Position >= 0 && vd.ReadByte() != (byte)'$') { fs.Position -= 2; }
                            }                                
                            else
                                if (fs.Position < delimPos)
                                    while (fs.Position < fs.Length && vd.ReadByte() != (byte)'$') { }*/
                        }
                    }
                    vd.Close();
                    MessageBox.Show(printedFigures.Count.ToString(), "", MessageBoxButton.OK, MessageBoxImage.Error);
                    /*foreach (Figure fig in printedFigures)
                        formatter.Serialize(fs, fig);*/
                }                
            }
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMoving = false;
            isResizing = false;
            isDrawing = false;
            /*chosenFig = null;
            chosenFigUI = null;*/
            moveFirst = true;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMoving = false;
            isResizing = false;
            isDrawing = false;
            drawFirst = true;
        }
    }
}
