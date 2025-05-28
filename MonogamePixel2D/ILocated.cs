using Microsoft.Xna.Framework;

namespace MonoGamePixel2D
{
    /// <summary>
    /// Represents an object with a location (integer).
    /// </summary>
    public interface ILocated
    {
        public Point Location { get; set; }
    }
}
