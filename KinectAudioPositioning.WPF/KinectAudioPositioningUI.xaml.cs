//Project: KinectAudioPosition (http://KinectAudioPosition.codeplex.com/)
//Filename: KinectAudioPositioningUI.xaml.cs
//Version: 20151201

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinectAudioPositioning.WPF
{

  /// <summary>
  /// MainWindow.xaml
  /// </summary>
  public partial class KinectAudioPositioningUI : UserControl, IDisposable
  {

    #region --- Constants ---

    protected const string TEXT_STATUS = "Beam = {0:F} (blue), Source = {1:F} (orange) angles in degrees";

    #endregion

    #region --- Fields ---

    protected KinectMicArray kinectMic;

    #endregion

    #region --- Initialization ---

    public KinectAudioPositioningUI()
    {
      InitializeComponent();
    }

    #endregion

    #region --- Cleanup ---

    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing) // Dispose managed state (managed objects)
          Cleanup();

        // Note: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // set large fields to null.

        disposedValue = true;
      }
    }

    // Note: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~KinectAudioPositioningUI() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // Note: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }

    public void Cleanup()
    {
      kinectMic.Dispose();
      kinectMic = null;
    }
    
    #endregion

    #region --- Properties ---

    public KinectMicArray KinectMicArray
    {
      get { return kinectMic; }
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
      if (!myCanvas.IsLoaded) return;

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
    /// Event handler to cater for UserControl loaded
    /// Construct KinectMicArray and draw contents
    /// </summary>
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      kinectMic = new KinectMicArray() { AngleMultiplier = -1.0 };
      kinectMic.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(kinectMic_PropertyChanged);
      DrawContents();
    }

    /// <summary>
    /// Event handler to carer for UserControl size changed
    /// </summary>
    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      DrawContents();
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

    #endregion

  }
}
