using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Kinect;

namespace KinectAudioPositioning
{
    class KinectMicArray : INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// PropertyChanged Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        // Audio Source Angle and Current Beam Angle
        public double SourceAngleProperty { get { return  _sourceAngle; }  }
        public double BeamAngleProperty { get { return  _beamAngle; }  }
        #endregion Properties

        #region Constructors
        // Constructor
        public KinectMicArray()
        {
            if ( KinectSensor.KinectSensors.Count == 0 ) {
                throw new Exception( "Kinectを接続してください" );
            }

            kinect = KinectSensor.KinectSensors[0];
            if ( kinect.Status != KinectStatus.Connected ) {
                throw new Exception( "Kinectや電源が正しく接続されているか確認してください" );
            }

            _sourceAngle = 0.0;
            _beamAngle = 0.0;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.WorkerReportsProgress = true;
            worker.RunWorkerAsync();
        }
        // Deconstructor
        ~KinectMicArray()
        {
            if (worker != null) this.Dispose();
        }
        // Dispose method
        public void Dispose()
        {
            worker.CancelAsync();
            worker.Dispose();
            worker = null;
        }
        #endregion Constructors

        private KinectSensor kinect = null;
        private BackgroundWorker worker = new BackgroundWorker();
        private double _sourceAngle;
        private double _beamAngle;
        private readonly int sleep = 100;
        // Convert radian (Kinect) to degree (WPF)
        private double RadToDeg(double rad)
        {
            return 180.0 * rad / Math.PI;
        }
        // Event handler from Background worker's Progress changed Event
        // NotifyPropertyChnaged cannot be called from BackgrounWorker Thread
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string s = e.UserState as string;
            if (s == "Beam")
            {
                NotifyPropertyChanged("BeamAngleProperty");
            }
            else
            {
                NotifyPropertyChanged("SourceAngleProperty");
            }
        }
        #region BackgroundWorker
        /// <summary>
        /// Get audio source position angle from Kinect Microphone Array
        /// </summary>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Initialize Kinect
            KinectAudioSource source = kinect.AudioSource;
            source.BeamAngleChanged +=
                new EventHandler<BeamAngleChangedEventArgs>( AudioSource_BeamAngleChanged );
            source.SoundSourceAngleChanged +=
                new EventHandler<SoundSourceAngleChangedEventArgs>( AudioSource_SoundSourceAngleChanged );
            kinect.Start();

            //Start capturing audio                               
            using (var audioStream = source.Start())
            {
                while (worker != null)
                {
                    Thread.Sleep(sleep);
                }
            }
        }

        /// <summary>
        /// Get Curretn Beam Angle 
        /// </summary>
        void AudioSource_BeamAngleChanged( object sender, BeamAngleChangedEventArgs e )
        {
            _beamAngle = -e.Angle;
            if ( worker != null ) {
                worker.ReportProgress( 0, "Beam" );
            }
        }

        /// <summary>
        /// Get Curretn Audio Source Angle 
        /// </summary>
        void AudioSource_SoundSourceAngleChanged( object sender, SoundSourceAngleChangedEventArgs e )
        {
            if ( e.ConfidenceLevel > 0.9 ) {
                _sourceAngle = -e.Angle;
                if ( worker != null ) {
                    worker.ReportProgress( 0, "Source" );
                }
            }
        }

        #endregion BackgroundWorker


    }
    
}
