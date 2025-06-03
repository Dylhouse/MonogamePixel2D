using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Creates a new <see cref="Vector2"/> from a magnitude and an angle.
        /// </summary>
        /// <param name="magnitude">The vector's magnitude (distance).</param>
        /// <param name="angle">The vector's angle in radians. "Angle" is the angle between south and the vector, going counterclockwise</param>
        /// <returns>The new <see cref="Vector2"/>.</returns>
        public static Vector2 FromPolar(float magnitude, float angle)
        {
            return new Vector2(magnitude * MathF.Cos(angle), magnitude * MathF.Sin(angle));
        }

        /// <summary>
        /// Returns a new <see cref="Vector2"/> with its values truncated.
        /// </summary>
        /// <param name="v1">The <see cref="Vector2"/> to truncate.</param>
        /// <returns>The truncated <see cref="Vector2"/>.</returns>
        public static Vector2 Truncate(Vector2 v1) => new((int)v1.X, (int)v1.Y);
    }
}
