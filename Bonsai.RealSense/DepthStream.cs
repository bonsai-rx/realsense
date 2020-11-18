using Intel.RealSense;
using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.RealSense
{
    [Description("Subscribes to the native stream of depth images in a RealSense device.")]
    public class DepthStream : Combinator<Device, VideoDataFrame>
    {
        public DepthStream()
        {
            Framerate = 60;
        }

        public int Framerate { get; set; }

        public override IObservable<VideoDataFrame> Process(IObservable<Device> source)
        {
            return source.SelectMany(device =>
            {
                var depthSensor = device.QuerySensors().First(sensor => sensor.Is(Extension.DepthSensor));
                var profile = depthSensor.StreamProfiles.First(p => p.Framerate == Framerate && p.Stream == Stream.Depth);
                return Observable.Create<VideoDataFrame>(observer =>
                {
                    depthSensor.Open(profile);
                    depthSensor.Start(frame =>
                    {
                        VideoDataFrame output;
                        using (var videoFrame = frame.As<VideoFrame>())
                        {
                            var image = new IplImage(new Size(videoFrame.Width, videoFrame.Height), IplDepth.U16, 1);
                            videoFrame.CopyTo(image.ImageData);
                            output = new VideoDataFrame(videoFrame, image);
                        }

                        observer.OnNext(output);
                    });
                    return Disposable.Create(() =>
                    {
                        depthSensor.Stop();
                        depthSensor.Close();
                    });
                });
            });
        }
    }
}
