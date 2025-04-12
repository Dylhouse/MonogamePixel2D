using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Assets.Graphics;

/// <summary>
/// Interface for things than can be drawn or drawn and updated, such as a sprite.
/// </summary>
public interface IDrawable
{
    /// <summary>
    /// Determines whether or not the sprite will be drawn when <c>Draw</c> is called.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Determines the depth at which the sprite will be drawn. Higher depths will display over lower depths.
    /// </summary>
    public float LayerDepth { get; set; }

    /// <summary>
    /// Draws the sprite.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Draw(GameTime gameTime);
}
