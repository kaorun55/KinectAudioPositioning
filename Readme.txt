KinectAudioPositioning
=======================================

Description
-----------

This project is a C# library and WPF demo program for Kinect for Windows SDK, providing easy access to Kinect sensor's Audio Positioning features.
It provides KinectMicArray class (implementing INotifyPropertyChanged).
This class exposes beam angle and estimated audio source angle with notification of the change.
These properties are binded to the angle of respective colored lines to show the change dynamically.

Change History
--------------

* 20151029
[George Birbilis / Zoomicon.com]
- Since original repository seem to be abandoned, forking (at http://github.com/birbilis/KinectAudioPositioning)
from Kaoru Nakamura's independent port to Kinect SDK 1.0 (at https://github.com/kaorun55/KinectAudioPositioning)
and replacing his code that was still using background workers (they don't seem to be needed in WPF and Kinect SDK 1.8)
- Split into separate KinectAudioPositioning library and KinectAudioPositioning.WPF demo application using that library

* 20151024
[George Birbilis / Zoomicon.com]
- Updated to Kinect SDK v1.8 (see https://zoomicon.wordpress.com/2015/10/24/howto-upgrade-kinect-audio-positioning-code-from-older-beta-sdk/)
- Centering window on screen
- Cleaned up and refactored XAML and code
- Changed Japanese to English in various auxiliary source files

* 20110706
[Hiroyuki Kawanishi / Evangelist @Microsoft Japan]
- Original version (http://KinectAudioPosition.codeplex.com)
