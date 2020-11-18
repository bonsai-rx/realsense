using Intel.RealSense;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Bonsai.RealSense
{
    [Description("Extracts the intrinsic camera parameters for the specified stream profile.")]
    public class GetIntrinsics : Transform<VideoStreamProfile, Intrinsics>
    {
        public override IObservable<Intrinsics> Process(IObservable<VideoStreamProfile> source)
        {
            return source.Select(profile => profile.GetIntrinsics());
        }
    }
}
