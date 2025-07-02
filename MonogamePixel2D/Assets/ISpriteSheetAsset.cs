using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Assets
{
    public interface ISpriteSheetAsset
    {
        /// <summary>
        /// Creates a new instance of this <see cref="ISpriteSheetAsset"/>.
        /// </summary>
        /// <param name="sheetTexture">Reference to the <see cref="Texture2D"/> of the spritesheet to be used for drawing.</param>
        /// <param name="sourceRectangle">The area of the asset's texture data on the <paramref name="sheetTexture"/>.</param>
        /// <param name="path">The directory that may contain any additional sidecar or JSON files for the asset.</param>
        /// <returns>A new instance of this <see cref="ISpriteSheetAsset"/> as an <see cref="object"/>.</returns>
        public abstract static object Load(Texture2D sheetTexture, Rectangle sourceRectangle, string path);
    }
}
