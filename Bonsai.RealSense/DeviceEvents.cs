using RealSense.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.RealSense
{
    public class DeviceEvents
    {
        public DeviceEvents(Device device)
        {
            Device = device;
        }

        public Device Device { get; private set; }

        public event FrameCallback DepthFrame;

        public event FrameCallback ColorFrame;

        public event FrameCallback InfraredFrame;

        public event FrameCallback Infrared2Frame;

        public event FrameCallback FisheyeFrame;

        internal void OnDepthFrame(Frame frame)
        {
            var handler = DepthFrame;
            if (handler != null)
            {
                handler(frame);
            }
        }

        internal void OnColorFrame(Frame frame)
        {
            var handler = ColorFrame;
            if (handler != null)
            {
                handler(frame);
            }
        }

        internal void OnInfraredFrame(Frame frame)
        {
            var handler = InfraredFrame;
            if (handler != null)
            {
                handler(frame);
            }
        }

        internal void OnInfrared2Frame(Frame frame)
        {
            var handler = Infrared2Frame;
            if (handler != null)
            {
                handler(frame);
            }
        }

        internal void OnFisheyeFrame(Frame frame)
        {
            var handler = FisheyeFrame;
            if (handler != null)
            {
                handler(frame);
            }
        }
    }
}
