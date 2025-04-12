using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePixel2D;

/// <summary>
/// Interface for objects than can be updated, usually with regard to the deltaTime.
/// </summary>
public interface IUpdatable
{
    /// <summary>
    /// Updates the object based on the elapsed time between frames.
    /// </summary>
    /// <param name="gameTime">Game state and time data.</param>
    public void Update(GameTime gameTime);
}