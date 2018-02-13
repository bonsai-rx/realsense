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
    [Description("Extracts the intrinsic camera parameters for the specified stream.")]
    public class GetStreamIntrinsics : Transform<Device, Intrinsics>
    {
        [Description("The stream for which to extract the intrinsic camera parameters.")]
        public Stream Stream { get; set; }

        public override IObservable<Intrinsics> Process(IObservable<Device> source)
        {
            return source.Select(device => device.GetStreamIntrinsics(Stream));
        }
    }
}
