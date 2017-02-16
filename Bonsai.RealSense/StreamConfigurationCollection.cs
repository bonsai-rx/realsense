using RealSense.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.RealSense
{
    public class StreamConfigurationCollection : KeyedCollection<Stream, StreamConfiguration>
    {
        protected override Stream GetKeyForItem(StreamConfiguration item)
        {
            return item.Stream;
        }
    }
}
