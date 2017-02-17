using OpenCV.Net;
using RealSense.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.RealSense
{
    [Description("Subscribes to the native stream of color images in a RealSense device.")]
    public class ColorStream : Combinator<DeviceEvents, IplImage>
    {
        public override IObservable<IplImage> Process(IObservable<DeviceEvents> source)
        {
            return source.SelectMany(device => Observable.FromEvent<FrameCallback, Frame>(
                handler => device.ColorFrame += handler,
                handler => device.ColorFrame -= handler))
                .Select(frame =>
                {
                    var size = new Size(frame.Width, frame.Height);
                    var image = new IplImage(size, IplDepth.U8, 3);
                    var frameHeader = new Mat(size, Depth.U8, 3, frame.FrameData, frame.Stride);
                    CV.Copy(frameHeader, image);
                    return image;
                });
        }
    }
}
