using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectAudioPositioning
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private KinectMicArray kinectMic;
        private Label myLabel;
        /// <summary>
        /// Event handler to care Window loaded
        /// Construct KinectMicArray and draw contents
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try {
                kinectMic = new KinectMicArray();
                kinectMic.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler( kinectMic_PropertyChanged );
                DrawContents();
            }
            catch ( Exception ex ) {
                MessageBox.Show( ex.Message );
                Close();
            }
        }
        /// <summary>
        /// Event handler to care KinectMicArray property changed
        /// Showing angles as number for debug
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void kinectMic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            KinectMicArray ka = sender as KinectMicArray;
            this.myLabel.Content = string.Format("Beam = {0:F}; Source = {1:F};  ", ka.BeamAngleProperty, ka.SourceAngleProperty); 
        }
        /// <summary>
        /// Draw all contents
        /// </summary>
        private void DrawContents()
        {
            myCanvas.Children.Clear();

            Image kinectImage = FindResource("KinectImage") as Image;
            kinectImage.RenderTransform = new TranslateTransform(myCanvas.ActualWidth/2 - kinectImage.Source.Width / 2, myCanvas.ActualHeight/4 - kinectImage.Source.Height);
            myCanvas.Children.Add(kinectImage);

            myLabel = new Label();
            myLabel.Width = myCanvas.ActualWidth ;
            myLabel.Height = 30;
            myLabel.Content = "Audio Positioning";
            myLabel.SetValue(Canvas.BottomProperty, 0.0);
            myLabel.FontSize = 18.0;
            myCanvas.Children.Add(myLabel);

            // Draw Base Beam Lines
            for (int i = -5; i < 6; i++)
            {
                myCanvas.Children.Add( DrawLine(Brushes.Green, 2.0, 10.0 * i));
            }
            // Draw Current Beam Line with binding
            myCanvas.Children.Add( DrawLine(Brushes.Blue, 4.0, "BeamAngleProperty"));
            // Draw Source Line with binding
            myCanvas.Children.Add( DrawLine(Brushes.Orange, 4.0, "SourceAngleProperty"));
        }
        /// <summary>
        /// Draw Line with Binding
        /// </summary>
        private Line DrawLine(Brush brush, double thickness, string bindingProperty)
        {
            Line l = DrawLine(brush, thickness, 0.0);
            RotateTransform r = new RotateTransform();
            Binding b = new Binding(bindingProperty);
            
            l.RenderTransform = r;

            r.CenterX = l.X1;
            r.CenterY = l.Y1;

            b.Source = kinectMic;
            b.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(r, RotateTransform.AngleProperty, b);

            return l;
        }
        /// <summary>
        /// Draw Line without Binding
        /// </summary>
        private Line DrawLine(Brush brush, double thickness, double angle)
        {
            Point s = new Point(myCanvas.ActualWidth / 2, myCanvas.ActualHeight / 4);
            Point e = new Point(myCanvas.ActualWidth / 2, myCanvas.ActualHeight); 
            Line l = new Line();
            l.X1 = s.X;
            l.Y1 = s.Y;
            l.X2 = e.X;
            l.Y2 = e.Y;
            l.Stroke = brush;
            l.StrokeThickness = thickness;
            l.RenderTransform = new RotateTransform(angle, s.X, s.Y);
            return l;
        }
        /// <summary>
        /// Event handler to care window size changed
        /// </summary>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (myCanvas.IsLoaded)  DrawContents();
        }
        /// <summary>
        /// Event handler to care window closing
        /// KinectMicArray object should be disposed in order to kill background worker 
        /// </summary>
        private void Window_Closing(object sender, EventArgs e)
        {
            kinectMic.Dispose();
        }
    }
}
