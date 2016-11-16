using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.RealSense
{
    public class RealSenseCapture : Source<RealSenseDataFrame>
    {
        public PXCMCapture.StreamType StreamType { get; set; }

        public override IObservable<RealSenseDataFrame> Generate()
        {
            return Observable.Create<RealSenseDataFrame>((observer, cancellationToken) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    using (var manager = PXCMSenseManager.CreateInstance())
                    {
                        var status = manager.EnableStream(StreamType, 0, 0);

                        status = manager.Init();
                        if (status != pxcmStatus.PXCM_STATUS_NO_ERROR)
                        {
                            throw new InvalidOperationException("Initialization failed.");
                        }

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            if (manager.AcquireFrame().IsError())
                            {
                                throw new InvalidOperationException("Acquisition error.");
                            }

                            var sample = manager.QuerySample();
                            var output = new RealSenseDataFrame(sample);
                            manager.ReleaseFrame();
                            observer.OnNext(output);
                        }
                    }
                },
                cancellationToken,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            });
        }
    }
}
