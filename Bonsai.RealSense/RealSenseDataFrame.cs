using OpenCV.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.RealSense
{
    public class RealSenseDataFrame
    {
        internal RealSenseDataFrame(PXCMCapture.Sample sample)
        {
            Color = FromPXCMImage(sample.color);
            Depth = FromPXCMImage(sample.depth);
            Ir = FromPXCMImage(sample.ir);
        }

        public IplImage Color { get; private set; }

        public IplImage Depth { get; private set; }

        public IplImage Ir { get; private set; }

        static void GetImageFormat(PXCMImage.PixelFormat format, out PXCMImage.PixelFormat dataFormat, out IplDepth depth, out int channels)
        {
            dataFormat = PXCMImage.PixelFormat.PIXEL_FORMAT_ANY;
            depth = IplDepth.U8;
            channels = 0;
            switch (format)
            {
                case PXCMImage.PixelFormat.PIXEL_FORMAT_ANY:
                    break;
                case PXCMImage.PixelFormat.PIXEL_FORMAT_DEPTH:
                case PXCMImage.PixelFormat.PIXEL_FORMAT_DEPTH_RAW:
                    depth = IplDepth.U16;
                    channels = 1;
                    break;
                case PXCMImage.PixelFormat.PIXEL_FORMAT_DEPTH_CONFIDENCE:
                    break;
                case PXCMImage.PixelFormat.PIXEL_FORMAT_DEPTH_F32:
                    depth = IplDepth.F32;
                    channels = 1;
                    break;
                case PXCMImage.PixelFormat.PIXEL_FORMAT_NV12:
                    break;
                case PXCMImage.PixelFormat.PIXEL_FORMAT_RGB24:
                    depth = IplDepth.U8;
                    channels = 3;
                    break;
                case PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32:
                    depth = IplDepth.U8;
                    channels = 4;
                    break;
                case PXCMImage.PixelFormat.PIXEL_FORMAT_Y16:
                    depth = IplDepth.U16;
                    channels = 1;
                    break;
                case PXCMImage.PixelFormat.PIXEL_FORMAT_Y8:
                    break;
                case PXCMImage.PixelFormat.PIXEL_FORMAT_Y8_IR_RELATIVE:
                    break;
                case PXCMImage.PixelFormat.PIXEL_FORMAT_YUY2:
                    dataFormat = PXCMImage.PixelFormat.PIXEL_FORMAT_RGB24;
                    depth = IplDepth.U8;
                    channels = 3;
                    break;
                default:
                    break;
            }
        }

        static IplImage FromPXCMImage(PXCMImage image)
        {
            if (image == null) return null;

            int channels;
            IplDepth depth;
            PXCMImage.PixelFormat dataFormat;
            Size size = new Size(image.info.width, image.info.height);
            GetImageFormat(image.info.format, out dataFormat, out depth, out channels);

            PXCMImage.ImageData data;
            image.AcquireAccess(PXCMImage.Access.ACCESS_READ, dataFormat, out data);

            var output = new IplImage(size, depth, channels);
            using (var header = new IplImage(size, depth, channels, data.planes[0]))
            {
                CV.Copy(header, output);
            }
            image.ReleaseAccess(data);
            return output;
        }
    }
}
