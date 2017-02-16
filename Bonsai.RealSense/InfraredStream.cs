using OpenCV.Net;
using RealSense.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.RealSense
{
    public class InfraredStream : Combinator<DeviceEvents, IplImage>
    {
        public override IObservable<IplImage> Process(IObservable<DeviceEvents> source)
        {
            return source.SelectMany(device => Observable.FromEvent<FrameCallback, Frame>(
                handler => device.InfraredFrame += handler,
                handler => device.InfraredFrame -= handler))
                .Select(frame =>
                {
                    var size = new Size(frame.Width, frame.Height);
                    var image = new IplImage(size, IplDepth.U8, 1);
                    var frameHeader = new Mat(size, Depth.U8, 1, frame.FrameData, frame.Stride);
                    CV.Copy(frameHeader, image);
                    return image;
                });
        }
    }
}
