﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Koga.Paint
{
    public class StrokeBuilder
    {
        internal static readonly float K = 0.6f;

        public static SKPath CreateCubicStroke(List<SKPoint> stroke, float k)
        {
            SKPath strokePath = new SKPath();

            var controls = CalculateCubicControls(stroke, k);

            if (controls != null)
            {
                for (int i = 1; i < controls.Count; i += 3)
                {
                    strokePath.MoveTo(controls[i - 1]);
                    strokePath.CubicTo(controls[i], controls[i + 1], controls[i + 2]);
                }
            }

            return strokePath;
        }

        public static SKPath CreateHandwriteStroke(List<SKPoint> stroke, float strokeWidth, float k, int expandTypes)
        {
            SKPath strokePath = new SKPath();

            var outlines = ExpandLinesV(stroke, strokeWidth, expandTypes);

            var controls = CalculateCubicControls(outlines.stroke1, k);

            if (controls != null)
            {
                strokePath.MoveTo(controls[0]);
                for (int i = 1; i < controls.Count; i += 3)
                {
                    strokePath.CubicTo(controls[i], controls[i + 1], controls[i + 2]);
                }

                controls = CalculateCubicControls(outlines.stroke2, k);
                controls.Reverse();

                for (int i = 1; i < controls.Count; i += 3)
                {
                    strokePath.CubicTo(controls[i], controls[i + 1], controls[i + 2]);
                }
            }

            strokePath.Close();

            
            return strokePath;
        }

        public static SKPath CreateSoftbrushStroke(List<SKPoint> stroke, float strokeWidth, float k)
        {
            SKPath strokePath = CreateCubicStroke(stroke, k);
            strokePath = GetPointsOnPath(strokePath, strokeWidth);

            return strokePath;
        }
        public static SKPath GetPointsOnPath(SKPath path, float strokeWidth, float precision = 1.0f)
        {
            SKPath strokePath = new SKPath();
            using (SKPathMeasure measure = new SKPathMeasure(path, false))
            {
                float lastLength = 0;
                float lastWidth = 1;

                do
                {
                    float length = measure.Length;
                    float t = length / 10;
                    t = 1f - (t < 0.95f ? t : 0.95f);
                    float w1 = strokeWidth * t;

                    float m = (w1 - lastWidth) / length;


                    for (float distance = 0; distance < length; distance += precision)
                    {
                        SKPoint point;
                        measure.GetPosition(distance, out point);
                        strokePath.AddCircle(point.X, point.Y, (m * distance + lastWidth) / 2);
                    }

                    lastLength = length;
                    lastWidth = w1;
                } while (measure.NextContour());
            }

            return strokePath;
        }

        //SKPath CreateCubicStroke1(List<SKPoint> stroke, float strokeWidth, float k)
        //{
        //    SKPath strokePath = new SKPath();

        //    //            var outlines = ExpandLines(stroke, strokeWidth);

        //    var controls = CalculateCubicControls(stroke, k);

        //    if (controls != null)
        //    {
        //        var outlines = ExpandLines(controls, strokeWidth);

        //        strokePath.MoveTo(outlines.stroke1[0]);
        //        for (int i = 1; i < outlines.stroke1.Count; i += 3)
        //        {
        //            strokePath.CubicTo(outlines.stroke1[i], outlines.stroke1[i + 1], outlines.stroke1[i + 2]);
        //        }

        //        //controls = CalculateCubicControls(outlines.stroke2, k);
        //        outlines.stroke2.Reverse();

        //        for (int i = 1; i < outlines.stroke2.Count; i += 3)
        //        {
        //            strokePath.CubicTo(outlines.stroke2[i], outlines.stroke2[i + 1], outlines.stroke2[i + 2]);
        //        }
        //    }

        //    strokePath.Close();

        //    return strokePath;
        //}


        public static (List<SKPoint> stroke1, List<SKPoint> stroke2) ExpandLines(List<SKPoint> stroke, float strokeWidth)
        {
            if (stroke.Count < 3)
            {
                return (new List<SKPoint>(), new List<SKPoint>());
            }

            List<SKPoint> stroke1 = new List<SKPoint>();
            List<SKPoint> stroke2 = new List<SKPoint>();

            stroke1.Add(stroke[0]);
            stroke2.Add(stroke[0]);

            for (int i = 1; i < stroke.Count - 1; i++)
            {
                var ret = ExpandPoints(stroke[i - 1], stroke[i], stroke[i + 1], strokeWidth);
                //if(i==1)
                //{
                //    stroke1.Add(ret.p0L);
                //    stroke2.Add(ret.p0R);
                //}
                stroke1.Add(ret.p1L);
                stroke2.Add(ret.p1R);
            }
            stroke1.Add(stroke[stroke.Count - 1]);
            stroke2.Add(stroke[stroke.Count - 1]);

            return (stroke1, stroke2);
        }

        public static (List<SKPoint> stroke1, List<SKPoint> stroke2) ExpandLinesV(List<SKPoint> stroke, float strokeWidth, int expandTypes)
        {
            if (stroke.Count < 3)
            {
                return (new List<SKPoint>(), new List<SKPoint>());
            }

            List<SKPoint> stroke1 = new List<SKPoint>();
            List<SKPoint> stroke2 = new List<SKPoint>();

            stroke1.Add(stroke[0]);
            stroke2.Add(stroke[0]);

            SKPoint v, p0, p1, p2;

            float w = strokeWidth;
            for (int i = 1; i < stroke.Count - 1; i++)
            {
                p0 = stroke[i - 1];
                p1 = stroke[i];
                p2 = stroke[i + 1];

                if (expandTypes == 1)
                {
                    v = stroke[i] - stroke[i - 1];
                    float t = v.Length / 10;
                    t = 1f - (t < 0.95f ? t : 0.95f);
                    w = strokeWidth * t;
                }
                else if (expandTypes == 2)
                {
                    //根据向量点积公式计算两条线的夹角的余弦
                    SKPoint v1 = p0 - p1;
                    SKPoint v2 = p2 - p1;

                    float cos = (v1.X * v2.X + v1.Y * v2.Y) / (v1.Length * v2.Length);
                    w = strokeWidth * (float)Math.Sqrt(1 - cos * cos) + 0.5f;
                }
                //w = w < (strokeWidth / 3) ? (strokeWidth / 3) : strokeWidth;

                var ret = ExpandPoints(p0, p1, p2, w);

                stroke1.Add(ret.p1L);
                stroke2.Add(ret.p1R);
            }
            stroke1.Add(stroke[stroke.Count - 1]);
            stroke2.Add(stroke[stroke.Count - 1]);

            return (stroke1, stroke2);
        }


        public static (SKPoint p0L, SKPoint p0R, SKPoint p1L, SKPoint p1R, SKPoint p2L, SKPoint p2R) ExpandPoints(SKPoint p0, SKPoint p1, SKPoint p2, float strokeWidth)
        {
            //计算p0p1和p1p2的方向向量
            SKPoint v1 = p1 - p0;
            SKPoint v2 = p2 - p1;

            if(v1.Length == 0)
            {
                Debug.WriteLine("v1.Length == 0");
            }

            if(v2.Length==0)
            {
                Debug.WriteLine("v2.Length == 0");
            }

            //计算p0p1和p1p2的单位法向量
            SKPoint n1 = new SKPoint(-v1.Y / v1.Length, v1.X / v1.Length);
            SKPoint n2 = new SKPoint(-v2.Y / v2.Length, v2.X / v2.Length);

            //沿着法向量方向移动strokeWidth/2，分别计算p01和p10的左右两个点
            SKPoint p10L = p1 + new SKPoint(n1.X * strokeWidth / 2, n1.Y * strokeWidth / 2);
            SKPoint p10R = p1 + new SKPoint(n1.X * -strokeWidth / 2, n1.Y * -strokeWidth / 2);

            SKPoint p01L = p0 + new SKPoint(n1.X * strokeWidth / 2, n1.Y * strokeWidth / 2);
            SKPoint p01R = p0 + new SKPoint(n1.X * -strokeWidth / 2, n1.Y * -strokeWidth / 2);

            //沿着法向量方向移动strokeWidth/2，分别计算p12和p21的左右两个点
            SKPoint p12L = p1 + new SKPoint(n2.X * strokeWidth / 2, n2.Y * strokeWidth / 2);
            SKPoint p12R = p1 + new SKPoint(n2.X * -strokeWidth / 2, n2.Y * -strokeWidth / 2);

            SKPoint p21L = p2 + new SKPoint(n2.X * strokeWidth / 2, n2.Y * strokeWidth / 2);
            SKPoint p21R = p2 + new SKPoint(n2.X * -strokeWidth / 2, n2.Y * -strokeWidth / 2);

            //计算p01p10和p12p21左右两条线的交点
            SKPoint p1L, p1R;

            float tm = (p10L.X - p01L.X) * (p21L.Y - p12L.Y) - (p10L.Y - p01L.Y) * (p21L.X - p12L.X);
            float tn = (p12L.X - p01L.X) * (p21L.Y - p12L.Y) - (p12L.Y - p01L.Y) * (p21L.X - p12L.X);
            float t;

            if (tm == 0)
            {
                p1L = p10L;
            }
            else
            {
                t = tn / tm;
                p1L = new SKPoint(p01L.X + (p10L.X - p01L.X) * t, p01L.Y + (p10L.Y - p01L.Y) * t);
            }

            tm = (p10R.X - p01R.X) * (p21R.Y - p12R.Y) - (p10R.Y - p01R.Y) * (p21R.X - p12R.X);
            tn = (p12R.X - p01R.X) * (p21R.Y - p12R.Y) - (p12R.Y - p01R.Y) * (p21R.X - p12R.X);

            if (tm == 0)
            {
                p1R = p10R;
            }
            else
            {
                t = tn / tm;
                p1R = new SKPoint(p01R.X + (p10R.X - p01R.X) * t, p01R.Y + (p10R.Y - p01R.Y) * t);
            }

            if (float.IsNaN(p01L.X))
            {
                p01L = p0;
                p01R = p0;

                p1L = p1;
                p1R = p1;
            }
            else if (float.IsNaN(p12L.X))
            {
                p1L = p1;
                p1R = p1;

                p21L = p2;
                p21R = p2;
            }

            //如果p01p10和p12p21的交点距离p1的距离大于strokeWidth*3/2，则将交点移动到p1的strokeWidth*3/2处, 抑制锐角
            SKPoint pLl = p1L - p1;
            SKPoint pRl = p1R - p1;
            float w = strokeWidth * 3 / 2;

            if (pLl.Length > w)
            {
                t = w / pLl.Length;
                p1L = new SKPoint(p1.X + pLl.X * t, p1.Y + pLl.Y * t);
            }

            if (pRl.Length > w)
            {
                t = w / pRl.Length;
                p1R = new SKPoint(p1.X + pRl.X * t, p1.Y + pRl.Y * t);
            }

            //Debug.WriteLine($"p1L:{(p1L-p1).Length},p1R:{(p1R-p1).Length}");

            return (p01L, p01R, p1L, p1R, p21L, p21R);
        }

        public static (SKPoint p1, SKPoint p2, SKPoint p3) CalculateCubicControl(SKPoint p0, SKPoint p1, SKPoint p2, float k)
        {
            //计算p0p1和p1p2的长度和比例
            float d01 = (float)Math.Sqrt((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y));
            float d12 = (float)Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            float d012 = d01 + d12;
            float t = d01 / d012;

            //计算p0p1和p1p2各自的中点，然后按p0p1和p1p2的比例计算这两个中点连线的c点
            SKPoint p01c = new SKPoint((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2);
            SKPoint p12c = new SKPoint((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
            SKPoint c = new SKPoint((p12c.X - p01c.X) * t + p01c.X, (p12c.Y - p01c.Y) * t + p01c.Y);

            //计算p01的控制点
            SKPoint o = p1 - c;
            SKPoint p01co = new SKPoint(c.X + (p01c.X - c.X) * k, c.Y + (p01c.Y - c.Y) * k);
            p01co.Offset(o);

            SKPoint p12co = new SKPoint(c.X + (p12c.X - c.X) * k, c.Y + (p12c.Y - c.Y) * k);
            p12co.Offset(o);

            return (p01co, p1, p12co);
        }

        public static List<SKPoint>? CalculateCubicControls(List<SKPoint> stroke, float k)
        {
            if (stroke.Count < 3)
            {
                return null;
            }

            List<SKPoint> controls = new List<SKPoint>();

            controls.Add(stroke[0]);
            controls.Add(stroke[0]);
            for (int i = 1; i < stroke.Count - 1; i++)
            {
                SKPoint p0 = stroke[i - 1];
                SKPoint p1 = stroke[i];
                SKPoint p2 = stroke[i + 1];

                //计算p0p1和p1p2的长度和比例
                float d01 = (float)Math.Sqrt((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y));
                float d12 = (float)Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
                float d012 = d01 + d12;
                float t = d01 / d012;

                //计算p0p1和p1p2各自的中点，然后按p0p1和p1p2的比例计算这两个中点连线的c点
                SKPoint p01c = new SKPoint((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2);
                SKPoint p12c = new SKPoint((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
                SKPoint c = new SKPoint((p12c.X - p01c.X) * t + p01c.X, (p12c.Y - p01c.Y) * t + p01c.Y);

                //计算p01的控制点
                SKPoint o = p1 - c;
                SKPoint p01co = new SKPoint(c.X + (p01c.X - c.X) * k, c.Y + (p01c.Y - c.Y) * k);
                p01co.Offset(o);

                SKPoint p12co = new SKPoint(c.X + (p12c.X - c.X) * k, c.Y + (p12c.Y - c.Y) * k);
                p12co.Offset(o);

                controls.Add(p01co);
                controls.Add(p1);
                controls.Add(p12co);
            }

            controls.Add(stroke[stroke.Count - 1]);
            controls.Add(stroke[stroke.Count - 1]);

            return controls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="baseWidth"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="thinRate">笔画变细的比率，0.1~0.5</param>
        /// <returns></returns>
        public static float ComputeStrokeWidthBySpeed(SKPoint p0, SKPoint p1, float baseWidth, float maxSpeed = 50, float thinRate = 0.2f)
        {
            if (thinRate > 0.5f || thinRate < 0.1f)
            {
                thinRate = 0.3f;
            }

            SKPoint v = p1 - p0;

            float speed = maxSpeed - v.Length;
            speed = (speed < 1 ? 1 : speed) / maxSpeed;
            float w = baseWidth * speed;
            w = w < baseWidth * thinRate ? baseWidth * thinRate : w;

            return w;
        }

        public static void MakeBrushStrokeSegment(SKPoint p0,float w0, SKPoint p1, float w1, SKPath path)
        {
            SKPoint v = p1 - p0;

            w0 *= 0.5f;
            w1 *= 0.5f;

            if (v.Length <= Math.Abs(w1 - w0))
            {
                if(w1 > w0)
                {
                    path.AddCircle(p1.X, p1.Y, w1, SKPathDirection.Clockwise);
                }

                return;
            }

            var points = ComputeTangentPoints(p0, w0, p1, w1);

            if (points.Count > 0)
            {
                path.MoveTo(points[0]);
                path.LineTo(points[1]);
                path.LineTo(points[2]);
                path.LineTo(points[3]);
                path.Close();
            }
            path.AddCircle(p1.X, p1.Y, w1, SKPathDirection.Clockwise);
        }

        public static SKPath CreateBrushStroke(List<SKPoint> stroke, float strokeWidth, float maxSpeed)
        {
            SKPath path = new SKPath();

            if (stroke.Count < 1)
            {
                return path;
            }

            float lastWidth = 1f;

            path.AddCircle(stroke[0].X, stroke[0].Y, lastWidth, SKPathDirection.Clockwise);

            for (int i = 1; i < stroke.Count; i++)
            {
                SKPoint p0 = stroke[i - 1];
                SKPoint p1 = stroke[i];
                SKPoint v = p1 - p0;

                float w = ComputeStrokeWidthBySpeed(p0, p1, strokeWidth, maxSpeed);

                //3个点以上，最后一个点变细
                if (stroke.Count>2 && i == stroke.Count - 1) w = 1f;

                MakeBrushStrokeSegment(p0, lastWidth, p1, w, path);

                lastWidth = w;
            }

            return path;
        }

        //计算两个圆的外公切线切点
        public static List<SKPoint> ComputeTangentPoints(SKPoint p1, float r1, SKPoint p2, float r2)
        {
            List<SKPoint> points = new List<SKPoint>();

            SKPoint dp = p2 - p1;

            if (dp.Length <= Math.Abs(r1 - r2))
            {
                return points;
            }

            double d = dp.Length;

            //圆心连线角度
            double theta = Math.Atan2(dp.Y, dp.X);

            //外公切线角
            double alpha = Math.Asin((r1 - r2) / dp.Length) - Math.PI * 0.5;

            //求外公切线切点
            points.Add(new SKPoint((float)(p1.X + r1 * Math.Cos(theta + alpha)), (float)(p1.Y + r1 * Math.Sin(theta + alpha))));
            points.Add(new SKPoint((float)(p2.X + r2 * Math.Cos(theta + alpha)), (float)(p2.Y + r2 * Math.Sin(theta + alpha))));
            points.Add(new SKPoint((float)(p2.X + r2 * Math.Cos(theta - alpha)), (float)(p2.Y + r2 * Math.Sin(theta - alpha))));
            points.Add(new SKPoint((float)(p1.X + r1 * Math.Cos(theta - alpha)), (float)(p1.Y + r1 * Math.Sin(theta - alpha))));

            return points;
        }

    }
}
