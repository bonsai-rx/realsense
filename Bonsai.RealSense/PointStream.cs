using OpenCV.Net;
using RealSense.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.RealSense
{
    public class PointStream : Combinator<DeviceEvents, Mat>
    {
        struct TexVertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;
        }

        public override IObservable<Mat> Process(IObservable<DeviceEvents> source)
        {
            return source.SelectMany(evts =>
            {
                var device = evts.Device;
                Extrinsics depthToColor;
                Intrinsics colorIntrinsics, depthIntrinsics;
                var colorStream = Observable.FromEvent<FrameCallback, Frame>(handler => evts.ColorFrame += handler, handler => evts.ColorFrame -= handler);
                var depthStream = Observable.FromEvent<FrameCallback, Frame>(handler => evts.DepthFrame += handler, handler => evts.DepthFrame -= handler);
                device.GetStreamIntrinsics(Stream.Color, out colorIntrinsics);
                device.GetStreamIntrinsics(Stream.Depth, out depthIntrinsics);
                device.GetExtrinsics(Stream.Depth, Stream.Color, out depthToColor);
                var pixelScale = new Vector2(colorIntrinsics.Width, colorIntrinsics.Height);
                var depthScale = device.DepthScale;


                int depthBufferWidth = 0;
                ushort[] depthBuffer = null;
                TexVertex[] depthPoints = null;
                var depthBufferStream = depthStream.Select(frame =>
                {
                    if (depthBuffer == null)
                    {
                        depthBufferWidth = frame.Width;
                        depthBuffer = new ushort[frame.Width * frame.Height];
                        depthPoints = new TexVertex[depthBuffer.Length];
                    }

                    var depthFrameHeader = new Mat(frame.Height, frame.Width, Depth.U16, 1, frame.FrameData, frame.Stride);
                    using (var bufferHeader = Mat.CreateMatHeader(depthBuffer, depthFrameHeader.Rows, depthFrameHeader.Cols, depthFrameHeader.Depth, depthFrameHeader.Channels))
                    {
                        CV.Copy(depthFrameHeader, bufferHeader);
                    }

                    return depthBuffer;
                });

                return depthBufferStream.Select(depth =>
                {
                    int pindex = 0;
                    for (int i = 0; i < depthBuffer.Length; i++)
                    {
                        Vector2 depthPixel, colorPixel;
                        depthPixel.X = i % depthBufferWidth;
                        depthPixel.Y = i / depthBufferWidth;
                        var depthValue = depth[i] * depthScale;
                        if (depthValue == 0) continue;

                        Vector3 depthPoint, colorPoint;
                        Intrinsics.DeprojectPoint(ref depthPixel, ref depthIntrinsics, depthValue, out depthPoint);
                        Extrinsics.TransformPoint(ref depthPoint, ref depthToColor, out colorPoint);
                        Intrinsics.ProjectPoint(ref colorPoint, ref colorIntrinsics, out colorPixel);
                        Vector2.Divide(ref colorPixel, ref pixelScale, out depthPoints[pindex].TexCoord);
                        depthPoints[pindex].Position = depthPoint;
                        pindex++;
                    }

                    return Mat.FromArray(depthPoints, pindex, 5, Depth.F32, 1);
                });
            });
        }
    }
}
