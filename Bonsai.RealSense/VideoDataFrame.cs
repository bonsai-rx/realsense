using Intel.RealSense;
using OpenCV.Net;

namespace Bonsai.RealSense
{
    public class VideoDataFrame
    {
        internal VideoDataFrame(VideoFrame frame, IplImage image)
        {
            Number = frame.Number;
            Sensor = frame.Sensor;
            Profile = frame.Profile.As<VideoStreamProfile>();
            Timestamp = frame.Timestamp;
            TimestampDomain = frame.TimestampDomain;
            Image = image;
        }

        public ulong Number { get; private set; }

        public Sensor Sensor { get; private set; }

        public VideoStreamProfile Profile { get; private set; }

        public double Timestamp { get; private set; }

        public TimestampDomain TimestampDomain { get; private set; }

        public IplImage Image { get; private set; }
    }
}
