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
    [Description("Gets the extrinsic transformation between the viewpoints of two different streams.")]
    public class GetExtrinsics : Transform<Device, Extrinsics>
    {
        public GetExtrinsics()
        {
            FromStream = Stream.Depth;
            ToStream = Stream.Color;
        }

        [Description("The stream coordinate space to transform from.")]
        public Stream FromStream { get; set; }

        [Description("The stream coordinate space to transform to.")]
        public Stream ToStream { get; set; }

        public override IObservable<Extrinsics> Process(IObservable<Device> source)
        {
            return source.Select(device => device.GetExtrinsics(FromStream, ToStream));
        }
    }
}
