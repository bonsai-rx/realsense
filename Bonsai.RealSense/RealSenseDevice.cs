using OpenCV.Net;
using RealSense.Net;
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
    [Description("Creates and connects to a RealSense device.")]
    public class RealSenseDevice : Source<DeviceEvents>
    {
        readonly StreamConfigurationCollection streams = new StreamConfigurationCollection();

        [Description("The index of the device.")]
        public int Index { get; set; }

        [Description("The collection of streams that should be enabled when connecting to the device.")]
        public StreamConfigurationCollection Streams
        {
            get { return streams; }
        }

        public override IObservable<DeviceEvents> Generate()
        {
            return Observable.Defer(() =>
            {
                var context = Context.Create();
                var device = context.Devices[Index];
                var deviceEvents = new DeviceEvents(device);
                foreach (var configuration in streams)
                {
                    FrameCallback callback;
                    switch (configuration.Stream)
                    {
                        case Stream.Depth: callback = deviceEvents.OnDepthFrame; break;
                        case Stream.Color: callback = deviceEvents.OnColorFrame; break;
                        case Stream.Infrared: callback = deviceEvents.OnInfraredFrame; break;
                        case Stream.Infrared2: callback = deviceEvents.OnInfrared2Frame; break;
                        case Stream.Fisheye: callback = deviceEvents.OnFisheyeFrame; break;
                        default: throw new InvalidOperationException("The specified stream type is not supported.");
                    }

                    device.EnableStream(configuration.Stream, configuration.Width, configuration.Height, configuration.Format, configuration.Framerate);
                    device.SetFrameCallback(configuration.Stream, callback);
                }

                device.Start();
                return Observable.Return(deviceEvents).Concat(Observable.Never<DeviceEvents>()).Finally(() =>
                {
                    device.Stop();
                    context.Dispose();
                });
            });
        }
    }
}
