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
    [Description("Creates and connects to a RealSense device.")]
    [WorkflowElementIcon(typeof(ElementCategory), "ElementIcon.Video")]
    public class RealSenseDevice : Source<Device>
    {
        [Description("The index of the device.")]
        public int Index { get; set; }

        public override IObservable<Device> Generate()
        {
            return Observable.Defer(() =>
            {
                var index = Index;
                var context = new Context();
                var devices = context.QueryDevices();
                if (devices.Count <= index)
                {
                    throw new InvalidOperationException("No RealSense device with the specified index was found.");
                }

                var device = devices[Index];
                return Observable.Return(device).Concat(Observable.Never<Device>()).Finally(() =>
                {
                    context.Dispose();
                });
            });
        }
    }
}
