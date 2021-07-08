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
    [Description("Subscribes to the native stream of color images in a RealSense device.")]
    public class ColorStream : Combinator<Device, VideoDataFrame>
    {
        public ColorStream()
        {
            Framerate = 60;
        }

        public int Framerate { get; set; }

        public override IObservable<VideoDataFrame> Process(IObservable<Device> source)
        {
            return source.SelectMany(device =>
            {
                var colorSensor = device.QuerySensors().First(sensor => sensor.Is(Extension.ColorSensor));
                var profile = colorSensor.StreamProfiles.First(
                    p => p.Framerate == Framerate &&
                    p.Stream == Stream.Color &&
                    p.Format == Format.Bgr8);
                return Observable.Create<VideoDataFrame>(observer =>
                {
                    colorSensor.Open(profile);
                    colorSensor.Start(frame =>
                    {
                        VideoDataFrame output;
                        using (var videoFrame = frame.As<VideoFrame>())
                        {
                            var image = new IplImage(new Size(videoFrame.Width, videoFrame.Height), IplDepth.U8, 3);
                            videoFrame.CopyTo(image.ImageData);
                            output = new VideoDataFrame(videoFrame, image);
                        }

                        observer.OnNext(output);
                    });
                    return Disposable.Create(() =>
                    {
                        colorSensor.Stop();
                        colorSensor.Close();
                    });
                });
            });
        }
    }
}
