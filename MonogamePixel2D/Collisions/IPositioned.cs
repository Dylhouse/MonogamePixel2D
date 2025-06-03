using Microsoft.Xna.Framework;

namespace MonoGamePixel2D.Collisions
{
    /// <summary>
    /// Represents an object with a position.
    /// </summary>
    public interface IPositioned
    {
        /// <summary>
        /// The current position of the object.
        /// </summary>
        public Vector2 Position { get; set; }
    }
}
