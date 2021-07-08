using Intel.RealSense;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.RealSense
{
    [Description("Gets the extrinsic transformation between the viewpoints of two different stream profiles.")]
    public class GetExtrinsics : Transform<Tuple<VideoStreamProfile, VideoStreamProfile>, Extrinsics>
    {
        public override IObservable<Extrinsics> Process(IObservable<Tuple<VideoStreamProfile, VideoStreamProfile>> source)
        {
            return source.Select(input =>
            {
                var from = input.Item1;
                var to = input.Item2;
                return from.GetExtrinsicsTo(to);
            });
        }
    }
}
