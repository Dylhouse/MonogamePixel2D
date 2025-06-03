using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Extensions
{
    public static class PointExtensions
    {
        public static Point Multiply(Point p1, Point p2) =>
            new Point(p1.X * p2.X, p1.Y * p2.Y);

        public static Point Multiply(Point p1, int scale) =>
            new Point(p1.X * scale, p1.Y * scale);

        public static Point Multiply(Point p1, float scale) =>
            new Point((int)((float)p1.X * scale), (int)((float)p1.Y * scale));

        public static Point Divide(Point p1, int scale) =>
            new Point(p1.X / scale, p1.Y / scale);

        public static Point Divide(Point p1, float scale) =>
            new Point((int)((float)p1.X / scale), (int)((float)p1.Y / scale));

        public static Point Divide(Point p1, Point p2) =>
            new Point(p1.X / p2.X, p1.Y / p2.Y);

        public static Point Mod(Point p1, int scale) =>
            new Point(p1.X % scale, p1.Y % scale);

        public static Point Mod(Point p1, float scale) =>
            new Point((int)((float)p1.X % scale), (int)((float)p1.Y % scale));

        public static Point Mod(Point p1, Point p2) =>
            new Point(p1.X % p2.X, p1.Y % p2.Y);
    }
}
