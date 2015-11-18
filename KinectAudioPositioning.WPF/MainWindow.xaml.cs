//Project: KinectAudioPosition (http://KinectAudioPosition.codeplex.com/)
//Filename: MainWindow.xaml.cs
//Version: 20151119

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinectAudioPositioning
{

  /// <summary>
  /// MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    #region --- Constants ---

    private const string TEXT_STATUS = "Beam = {0:F} (blue), Source = {1:F} (orange) angles in degrees";

    #endregion

    #region --- Fields ---

    private KinectMicArray kinectMic;

    #endregion

    #region --- Initialization ---

    public MainWindow()
    {
      InitializeComponent();
    }

    #endregion

    #region --- Methods ---

    /// <summary>
    /// Visualize the beam base lines.
    /// </summary>
    private void DrawBeamBaseLines()
    {
      for (int i = -5; i < 6; i++)
        myCanvas.Children.Add(DrawLine(Brushes.Green, 2.0, 10.0 * i));
    }

    /// <summary>
    /// Visualizes source and beam angles
    /// </summary>
    private void DrawContents()
    {
      myCanvas.Children.Clear();

      Image kinectImage = FindResource("KinectImage") as Image;
      kinectImage.RenderTransform = new TranslateTransform(myCanvas.ActualWidth / 2 - kinectImage.Source.Width / 2, myCanvas.ActualHeight / 4 - kinectImage.Source.Height);
      myCanvas.Children.Add(kinectImage);

      DrawBeamBaseLines();
      // Draw Current Beam Line with binding
      myCanvas.Children.Add(DrawLine(Brushes.Blue, 4.0, "BeamAngle"));
      // Draw Source Line with binding
      myCanvas.Children.Add(DrawLine(Brushes.Orange, 4.0, "SourceAngle"));
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

    #endregion

    #region --- Events ---

    /// <summary>
    /// Event handler to care Window loaded
    /// Construct KinectMicArray and draw contents
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      kinectMic = new KinectMicArray() { AngleMultiplier = -1.0 };
      kinectMic.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(kinectMic_PropertyChanged);
      DrawContents();
    }

    /// <summary>
    /// Event handler to care window closing
    /// KinectMicArray object should be disposed in order to release hardware resources
    /// </summary>
    private void Window_Closing(object sender, EventArgs e)
    {
      kinectMic.Dispose();
    }

    /// <summary>
    /// Event handler to care KinectMicArray property changed
    /// Showing angles as number for debug
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void kinectMic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      myLabel.Content = string.Format(TEXT_STATUS, kinectMic.BeamAngle, kinectMic.SourceAngle);
      DrawContents();
    }

    /// <summary>
    /// Event handler to care window size changed
    /// </summary>
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (myCanvas.IsLoaded)
        DrawContents();
    }

    #endregion

  }
}
