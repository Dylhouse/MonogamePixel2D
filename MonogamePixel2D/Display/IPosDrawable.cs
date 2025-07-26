using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Display
{
    public interface IPosDrawable
    {
        /// <summary>
        /// Draws the sprite at the given position.
        /// </summary>
        /// <param name="gameTime">Elapsed time between frames; could be used for an animated sprite, for example.</param>
        /// <param name="position">Drawing position.</param>
        public void Draw(GameTime gameTime, Point position);
    }
}
