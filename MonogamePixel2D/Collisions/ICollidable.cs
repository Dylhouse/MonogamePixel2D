using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Collisions;

/// <summary>
/// Allows an object to query collisions with other ICollidables.
/// </summary>
public interface ICollidable
{
    /// <summary>
    /// Returns whether the other collidable intersects with this collidable.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool CollidesWith(ICollidable other);

    /// <summary>
    /// Returns whether this collider intersects the given rectangle.
    /// </summary>
    /// <param name="rect">Rectangle to check collision with.</param>
    /// <returns></returns>
    public bool CollidesWithRect(Rectangle rect);
}
