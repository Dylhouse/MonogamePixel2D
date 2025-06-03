using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Extensions
{
    public static class Pixel
    {
        private static Texture2D? _texture;

        /// <summary>
        /// Gets a 1x1 white texture; a white pixel. This can be used for particles, rectangles, etc.
        /// The texture is cached.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch that may be used to create the texture if it has not already been created.</param>
        /// <returns>A 1x1 white texture; a white pixel.</returns>
        public static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _texture.SetData([Color.White]);
            }

            return _texture;
        }
    }
}
