using Microsoft.Xna.Framework;
using MonoGamePixel2D.Collisions;
using System.Collections.Generic;

namespace ShotgunGame.Extended;

public static class MathExtended
{
    /// <summary>
    /// Calculates a new <see cref="Vector2"/> with the component-wise minimum values from two given
    /// <see cref="Vector2"/> values.
    /// </summary>
    /// <param name="first">The first Vector2 value.</param>
    /// <param name="second">The second Vector2 value.</param>
    /// <returns>
    /// The calculated <see cref="Vector2"/> value with the component-wise minimum values.</returns>
    public static Vector2 CalculateMinimumVector2(Vector2 first, Vector2 second)
    {
        return new Vector2
        {
            X = first.X < second.X ? first.X : second.X,
            Y = first.Y < second.Y ? first.Y : second.Y
        };
    }

    /// <summary>
    /// Calculates a new <see cref="Vector2"/> with the component-wise minimum values from two given
    /// <see cref="Vector2"/> values.
    /// </summary>
    /// <param name="first">The first Vector2 value.</param>
    /// <param name="second">The second Vector2 value.</param>
    /// <param name="result">
    /// When this method returns, contains the calculated <see cref="Vector2"/> value with the component-wise minimum
    /// values. This parameter is passed uninitialized.
    /// </param>
    public static void CalculateMinimumVector2(Vector2 first, Vector2 second, out Vector2 result)
    {
        result.X = first.X < second.X ? first.X : second.X;
        result.Y = first.Y < second.Y ? first.Y : second.Y;
    }

    /// <summary>
    /// Calculates a new <see cref="Vector2"/> with the component-wise minimum values from two given
    /// <see cref="Vector2"/> values.
    /// </summary>
    /// <param name="first">The first Vector2 value.</param>
    /// <param name="second">The second Vector2 value.</param>
    /// <returns>The calculated <see cref="Vector2"/> value with the component-wise maximum values.</returns>
    public static Vector2 CalculateMaximumVector2(Vector2 first, Vector2 second)
    {
        return new Vector2
        {
            X = first.X > second.X ? first.X : second.X,
            Y = first.Y > second.Y ? first.Y : second.Y
        };
    }

    /// <summary>
    /// Calculates a new <see cref="Vector2"/> with the component-wise  values from two given
    /// <see cref="Vector2"/> values.
    /// </summary>
    /// <param name="first">The first Vector2 value.</param>
    /// <param name="second">The second Vector2 value.</param>
    /// <param name="result">
    /// When this method returns, contains the calculated <see cref="Vector2"/> value with the component-wise maximum
    /// values. This parameter is passed uninitialized.
    /// </param>
    public static void CalculateMaximumVector2(Vector2 first, Vector2 second, out Vector2 result)
    {
        result.X = first.X > second.X ? first.X : second.X;
        result.Y = first.Y > second.Y ? first.Y : second.Y;
    }

    /// <summary>
    /// Tries to get the point of intersection between the <paramref name="colliders"/> and the <paramref name="line"/>
    /// closest to the first point of the <paramref name="line"/>.
    /// </summary>
    /// <param name="colliders">The <see cref="Rectangle"/>s to check intersections with.</param>
    /// <param name="line">The <see cref="Line"/> to check intersections with.</param>
    /// <param name="intersection">The position of the point of intersection. Will be <see cref="Vector2"/> <see langword="default"/> if none is found.</param>
    /// <returns><see langword="true"/> if an intersection is found, otherwise <see langword="false"/>.</returns>
    public static bool TryGetIntersection(IEnumerable<RectangleF> colliders, Line line, out Vector2 intersection)
    {
        intersection = default;

        bool foundIntersection = false;
        float minDist = float.MaxValue;
        foreach (var collider in colliders)
        {
            if (collider.TryGetFirstEdgeIntersection(line, out var intersectionPos))
            {
                var dist = Vector2.DistanceSquared(line.Start, intersectionPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    intersection = intersectionPos;

                    foundIntersection = true;
                }
            }
        }

        return foundIntersection;
    }
}