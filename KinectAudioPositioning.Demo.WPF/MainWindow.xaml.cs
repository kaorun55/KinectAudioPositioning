//Project: KinectAudioPosition (http://KinectAudioPosition.codeplex.com/)
//Filename: MainWindow.xaml.cs
//Version: 20151201

using System;
using System.Windows;

namespace KinectAudioPositioning.Demo.WPF
{

  /// <summary>
  /// MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    #region --- Initialization ---

    public MainWindow()
    {
      InitializeComponent();
    }

    #endregion

    #region --- Events ---

    /// <summary>
    /// Event handler to care window closing
    /// KinectMicArray object should be disposed in order to release hardware resources
    /// </summary>
    private void Window_Closing(object sender, EventArgs e)
    {
      KinectAudioPos.Cleanup(); //probably not needed if WPF calls Dispose automatically on controls that implement IDisposable, but to be safe call Dispose anyway (it ignores subsequent calls)
    }

    #endregion

  }
}
