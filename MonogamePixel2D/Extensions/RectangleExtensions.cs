﻿using Microsoft.Xna.Framework;
using MonoGamePixel2D.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Extensions
{
    public static class RectangleExtensions
    {
        /// <summary>
        /// Gets the corresponding property value of the rectangle based on the given index.
        /// 0 => <see cref="Rectangle.X"/>, 1 => <see cref="Rectangle.Y"/>, 2 => <see cref="Rectangle.Width"/>, 3 => <see cref="Rectangle.Height"/>.
        /// </summary>
        /// <param name="rectangle">The rectangle to index.</param>
        /// <param name="propertyIndex">The index of the proprty</param>
        /// <returns>The <see cref="int"/> value of the property.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int GetProperty(this Rectangle rectangle, int propertyIndex) =>
            propertyIndex switch
            {
                0 => rectangle.X,
                1 => rectangle.Y,
                2 => rectangle.Width,
                3 => rectangle.Height,
                _ => throw new ArgumentException("Property index must be 0-3, inclusive"),
            };

        public static Rectangle Multiply(Rectangle rectangle1, int scale) =>
            new(rectangle1.X * scale, rectangle1.Y * scale, rectangle1.Width * scale, rectangle1.Height * scale);

        /// <summary>
        /// Attempts to find the first <see cref="Vector2"/> intersection point between <paramref name="rectangle1"/> and <paramref name="line"/>.
        /// </summary>
        /// <param name="rectangle1">The <see cref="Rectangle"/> to poll an intersection with.</param>
        /// <param name="line">The <see cref="Line"/> to poll an intersection with. The direction is important; <paramref name="intersection"/> 
        /// will be the <b>first</b> intersection point found going from the line's start to finish.</param>
        /// <param name="intersection">The point of intersection. Will be <see cref="Vector2"/> <see langword="default"/> if none is found.</param>
        /// <returns></returns>
        public static bool TryGetIntersection(this Rectangle rectangle1, Line line, out Vector2 intersection)
        {
            intersection = default;

            Span<Line> edges =
            [
                new Line(rectangle1.Left, rectangle1.Top, rectangle1.Left, rectangle1.Bottom), // top-left bottom-left
                new Line(rectangle1.Left, rectangle1.Top, rectangle1.Right, rectangle1.Top), // top-left top-right
                new Line(rectangle1.Left, rectangle1.Bottom, rectangle1.Right, rectangle1.Bottom), // bottom-left bottom-right
                new Line(rectangle1.Right, rectangle1.Top, rectangle1.Right, rectangle1.Bottom), // top-right bottom-right
            ];

            bool intersectionFound = false;
            float dist = float.MaxValue;

            foreach (var edge in edges)
            {
                if (Line.TryGetIntersection(line, edge, out var newIntersection))
                {
                    var length = Vector2.DistanceSquared(line.Start, newIntersection);
                    if (length < dist)
                    {
                        intersection = newIntersection;
                        dist = length;

                        intersectionFound = true;
                    }
                }
            }

            return intersectionFound;
        }
    }
}
