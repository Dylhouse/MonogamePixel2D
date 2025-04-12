using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Collisions;
public class RectangleCollider : ICollidable
{
    /// <summary>
    /// The rectangle hitbox.
    /// </summary>
    public Rectangle CollideRect { get; set; }

    public bool CollidesWith(ICollidable other)
    {
        return other.CollidesWithRect(CollideRect);
    }

    public bool CollidesWithRect(Rectangle rect)
    {
        return rect.Intersects(CollideRect);
    }
}
