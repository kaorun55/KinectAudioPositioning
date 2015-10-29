//Project: KinectAudioPosition (http://KinectAudioPosition.codeplex.com/)
//Filename: KinectMicArray.cs
//Version: 20151024

/*
Note: Ported to Kinect SDK v1.8 by George Birbilis / Zoomicon.com - At older Kinect Beta SDK:
  - could instantiate a KinectAudioSource class directly instead of having to get it from a started KinectSensor, plus the object was disposable so one could place it in "using" clause
  - could do audioSource.SystemMode = SystemMode.OptibeamArrayOnly
  - audioSource.BeamAngleChanged was called BeamChanged
  - audioSource.SoundSourceAngleChanged probably didn't exist (have seen older code use a BackgroundWorker and do polling)
  - audioSource.SoundSourceAngleConfidence was called SoundSourcePositionConfidence
  - audioSource.SoundSourceAngle was called SoundSourcePosition
  - audioSource.BeamAngle and audioSource.SoundSourceAngle (then called SoundSourcePosition) were returning radians instead of degrees (so one usually had to convert to degrees using 180.0 * rad / Math.PI)
*/

using Microsoft.Kinect;
using System;
using System.ComponentModel;
using System.IO;

namespace KinectAudioPositioning
{
  class KinectMicArray : INotifyPropertyChanged
  {

    #region --- Constants ---

    public const string PROPERTY_SOURCE_ANGLE = "SourceAngle";
    public const string PROPERTY_BEAM_ANGLE = "BeanAngle";

    #endregion

    #region --- Fields ---

    private KinectSensor sensor; //= null
    private KinectAudioSource audioSource; //=null
    private double _sourceAngle; //=0.0
    private double _beamAngle; //=0.0
    private double _angleMultiplier = 1.0;

    #endregion

    #region --- Properties ---

    private void NotifyPropertyChanged(String info)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(info));
    }

    /// <summary>
    /// Audio Source Angle
    /// </summary>
    public double SourceAngle {
      get { return _sourceAngle; }
    }

    /// <summary>
    /// Current Beam Angle
    /// </summary>
    public double BeamAngle {
      get { return _beamAngle; }
    }

    public double AngleMultiplier
    {
      get { return _angleMultiplier; }
      set { _angleMultiplier = value; }
    }

    #endregion Properties

    #region --- Initialization ---

    // Constructor
    public KinectMicArray()
    {
      sensor = StartKinectSensor(); //must do before trying to start the audio source
      audioSource = StartKinectAudioSource();
    }

    #endregion

    #region --- Cleanup ---

    /// <summary>
    /// Destructor
    /// </summary>
    ~KinectMicArray()
    {
      Dispose();
    }

    /// <summary>
    /// Dispose method
    /// </summary>
    public void Dispose()
    {
      if (audioSource != null)
      {
        audioSource.Stop(); //not really needed since we'll call Stop on the sensor itself
        audioSource = null;
      }

      if (sensor != null)
      {
        sensor.Stop();
        sensor.Dispose();
        sensor = null;
      }
    }

    #endregion

    #region --- Methods ---

    /// <summary>
    /// Looks through all connected Kinect sensors and tries to start one.
    /// </summary>
    /// <returns>Returns the first sensor that can be started succesfully, else null.</returns>
    public static KinectSensor StartKinectSensor()
    {
      foreach (KinectSensor sensor in KinectSensor.KinectSensors)
        if (sensor.Status == KinectStatus.Connected)
          try
          {
            sensor.Start(); // Start the sensor!
            return sensor; // return if started successfully
          }
          catch (IOException) // Some other application is streaming from the same Kinect sensor
          {
            //NOP
          }

      return null;
    }

    /// <summary>
    /// Starts the Kinect Audio Source.
    /// </summary>
    /// <returns></returns>
    public KinectAudioSource StartKinectAudioSource()
    {
      if (sensor == null)
        return null;

      KinectAudioSource source = sensor.AudioSource;
      source.SoundSourceAngleChanged += audio_SoundSourceAngleChanged;
      source.BeamAngleChanged += audio_BeamAngleChanged;
      source.Start();
      return source;
    }

    #endregion

    #region --- Events ---

    /// <summary>
    /// PropertyChanged Event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Handle SoundSourceAngleChanged event
    /// </summary>
    private void audio_SoundSourceAngleChanged(object sender, SoundSourceAngleChangedEventArgs e)
    {
      if (e.ConfidenceLevel > 0.9)
      {
        _sourceAngle = _angleMultiplier * e.Angle;
        NotifyPropertyChanged(PROPERTY_SOURCE_ANGLE);
      }
    }

    /// <summary>
    /// Handle BeamAngleChanged event
    /// </summary>
    private void audio_BeamAngleChanged(object sender, BeamAngleChangedEventArgs e)
    {
      _beamAngle = _angleMultiplier * e.Angle;
      NotifyPropertyChanged(PROPERTY_BEAM_ANGLE);
    }

    #endregion
  }

}
