using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.UI;
using Windows.UI;

namespace WriteBoard
{
    public static class DrawingHelper
    {
        public static void DrawCurve(this CanvasDrawingSession drawingSession, Vector2[] points, Color color, float width)
        {
            Debug.Assert(points != null && drawingSession != null);

            Vector2[] cp1, cp2;

            if (CalculateControlPoint(points, out cp1, out cp2))
            {
                using (var pathBuilder = new CanvasPathBuilder(drawingSession))
                {
                    pathBuilder.BeginFigure(points[0]);
                    for (int i = 1; i < points.Length; i++)
                    {
                        pathBuilder.AddCubicBezier(cp1[i - 1], cp2[i - 1], points[i]);
                    }
                    pathBuilder.EndFigure(CanvasFigureLoop.Open);

                    drawingSession.DrawGeometry(CanvasGeometry.CreatePath(pathBuilder), color, width);
                }
            }
        }

        static bool CalculateControlPoint(Vector2[] rawPointVector, out Vector2[] firstControlPointVector, out Vector2[] secondControlPointVector)
        {
            if (rawPointVector == null || rawPointVector.Length < 2)
            {
                firstControlPointVector = null;
                secondControlPointVector = null;

                return false;
            }

            int pointCount = rawPointVector.Length - 1;

            if (pointCount == 1)
            {
                firstControlPointVector = new Vector2[1];
                firstControlPointVector[0].X = (2 * rawPointVector[0].X + rawPointVector[1].X) / 3;
                firstControlPointVector[0].Y = (2 * rawPointVector[0].Y + rawPointVector[1].Y) / 3;

                secondControlPointVector = new Vector2[1];
                secondControlPointVector[0].X = 2 * firstControlPointVector[0].X - rawPointVector[0].X;
                secondControlPointVector[0].Y = 2 * firstControlPointVector[0].Y - rawPointVector[0].Y;

                return true;
            }

            double[] rhs = new double[pointCount];
            for (int i = 1; i < pointCount; ++i)
            {
                rhs[i] = 4 * rawPointVector[i].X + 2 * rawPointVector[i + 1].X;
            }

            rhs[0] = rawPointVector[0].X + 2 * rawPointVector[1].X;
            rhs[pointCount - 1] = (8 * rawPointVector[pointCount - 1].X + rawPointVector[pointCount].X) / 2.0;

            double[] x = GetFirstControlPoints(rhs);

            for (int i = 1; i < pointCount - 1; ++i)
            {
                rhs[i] = 4 * rawPointVector[i].Y + 2 * rawPointVector[i + 1].Y;
            }

            rhs[0] = rawPointVector[0].Y + 2 * rawPointVector[1].Y;
            rhs[pointCount - 1] = (8 * rawPointVector[pointCount - 1].Y + rawPointVector[pointCount].Y) / 2.0;

            double[] y = GetFirstControlPoints(rhs);

            firstControlPointVector = new Vector2[pointCount];
            secondControlPointVector = new Vector2[pointCount];

            for (int i = 0; i < pointCount; ++i)
            {
                // Second control point
                firstControlPointVector[i].X = (float)x[i];
                firstControlPointVector[i].Y = (float)y[i];
                if (i < pointCount - 1)
                {
                    secondControlPointVector[i].X = (float)(2 * rawPointVector[i + 1].X - x[i + 1]);
                    secondControlPointVector[i].Y = (float)(2 * rawPointVector[i + 1].Y - y[i + 1]);
                }
                else
                {
                    secondControlPointVector[i].X = (float)((rawPointVector[pointCount].X + x[pointCount - 1]) / 2);
                    secondControlPointVector[i].Y = (float)((rawPointVector[pointCount].Y + y[pointCount - 1]) / 2);
                }
            }

            return true;
        }

        static double[] GetFirstControlPoints(double[] rhs)
        {
            Debug.Assert(rhs != null);

            int n = rhs.Length;
            double[] x = new double[n];
            double[] tmp = new double[n];

            double b = 2.0;
            x[0] = rhs[0] / b;

            for (int i = 1; i < n; ++i) // Decomposition and forward substitution.
            {
                tmp[i] = 1 / b;
                b = (i < n - 1 ? 4.0 : 3.5) - tmp[i];
                x[i] = (rhs[i] - x[i - 1]) / b;
            }

            for (int i = 1; i < n; ++i)
            {
                x[n - i - 1] -= tmp[n - i] * x[n - i]; // Backsubstitution.
            }

            return x;
        }

    }
}
