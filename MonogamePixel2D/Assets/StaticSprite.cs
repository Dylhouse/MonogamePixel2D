using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePixel2D.Assets.Graphics;
using MonoGamePixel2D.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Assets
{
    /// <summary>
    /// Class for a static (non-animated) sprite that can be drawn with various parameters.
    /// </summary>
    public class StaticSprite : ISpriteSheetAsset, ILoadableAsset
    {
        /// <summary>
        /// The texture used by this in sprite in all of the draw methods.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// The rectangular region that this sprite will draw from its <see cref="Texture"/>.
        /// </summary>
        public Rectangle SourceRectangle { get; set; }


        /// <inheritdoc/>
        public static object Load(Texture2D texture, Rectangle sourceRectangle, string path) =>
            new StaticSprite(texture, sourceRectangle);

        /// <inheritdoc/>
        public static object Load(ContentManager content, string contentPath, string path) =>
            new StaticSprite(content.Load<Texture2D>(contentPath));

        /// <inheritdoc/>
        public static void Unload(ContentManager content, string contentPath) =>
            content.UnloadAsset(contentPath);

        /// <summary>
        /// Constructs a new <see cref="StaticSprite"/> with no rectangle bounds for drawing.
        /// This sprite will draw the entire <paramref name="texture"/> when drawing.
        /// </summary>
        /// <param name="texture">The sprite's texture.</param>
        public StaticSprite(Texture2D texture)
        {
            Texture = texture;
            SourceRectangle = texture.Bounds;
        }
        
        /// <summary>
        /// Constructs a new <see cref="StaticSprite"/> that draws a region <paramref name="sourceRectangle"/> from the
        /// <paramref name="texture"/>.
        /// </summary>
        /// <param name="texture"><inheritdoc cref="StaticSprite(Texture2D)"/></param>
        /// <param name="sourceRectangle">The rectangle region of the <paramref name="texture"/> that will be drawn when <c>Draw</c> is called.</param>
        public StaticSprite(Texture2D texture, Rectangle sourceRectangle)
        {
            Texture = texture;
            SourceRectangle = sourceRectangle;
        }

        /// <summary>
        /// Submits the texture to the spritebatch as a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The <c>SpriteBatch</c> to submit the sprite to for drawing.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>   
        /// <param name="color">A color mask.</param>
        public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color)
        {
            spriteBatch.Draw(Texture, destinationRectangle, SourceRectangle, color);
        }

        /// <summary>
        /// Submits the texture to the spritebatch as a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The <c>SpriteBatch</c> to submit the sprite to for drawing.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">A color mask.</param>
        public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            if (!sourceRectangle.HasValue)
            {
                spriteBatch.Draw(Texture, destinationRectangle, SourceRectangle, color);
                return;
            }

            spriteBatch.Draw(Texture, destinationRectangle, SourceRectangle.RelativeIntersection(sourceRectangle!.Value), color);
        }

        /// <summary>
        /// Submits the texture to the spritebatch as a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The <c>SpriteBatch</c> to submit the sprite to for drawing.</param>
        /// <param name="destinationRectangle">The drawing bounds on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">A color mask.</param>
        /// <param name="rotation">A rotation of this sprite.</param>
        /// <param name="origin">Center of the rotation. 0,0 by default.</param>
        /// <param name="effects">Modificators for drawing. Can be combined.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color,
            float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            if (!sourceRectangle.HasValue)
            {
                spriteBatch.Draw(Texture, destinationRectangle, SourceRectangle, color, rotation, origin, effects, layerDepth);
                return;
            }

            spriteBatch.Draw(Texture, destinationRectangle, SourceRectangle.RelativeIntersection(sourceRectangle!.Value), color, rotation, origin, effects, layerDepth);
        }

        /// <summary>
        /// Submits the texture to the spritebatch as a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The <c>SpriteBatch</c> to submit the sprite to for drawing.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="color">A color mask.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            spriteBatch.Draw(Texture, position, SourceRectangle, color);
        }

        /// <summary>
        /// Submits the texture to the spritebatch as a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The <c>SpriteBatch</c> to submit the sprite to for drawing.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">A color mask.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            if (!sourceRectangle.HasValue)
            {
                spriteBatch.Draw(Texture, position, SourceRectangle, color);
                return;
            }

            spriteBatch.Draw(Texture, position, SourceRectangle.RelativeIntersection(sourceRectangle!.Value), color);
        }

        /// <summary>
        /// Submits the texture to the spritebatch as a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The <c>SpriteBatch</c> to submit the sprite to for drawing.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">A color mask.</param>
        /// <param name="rotation">A rotation of this sprite.</param>
        /// <param name="origin">Center of the rotation. 0,0 by default.</param>
        /// <param name="scale">A scaling of this sprite.</param>
        /// <param name="effects">Modificators for drawing. Can be combined.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? sourceRectangle, Color color,
            float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            if (!sourceRectangle.HasValue)
            {
                spriteBatch.Draw(Texture, position, SourceRectangle, color, rotation, origin, scale, effects, layerDepth);
                return;
            }

            spriteBatch.Draw(Texture, position, SourceRectangle.RelativeIntersection(sourceRectangle!.Value), color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Submits the texture to the spritebatch as a sprite for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The <c>SpriteBatch</c> to submit the sprite to for drawing.</param>
        /// <param name="position">The drawing location on screen.</param>
        /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
        /// <param name="color">A color mask.</param>
        /// <param name="rotation">A rotation of this sprite.</param>
        /// <param name="origin">Center of the rotation. 0,0 by default.</param>
        /// <param name="scale">A scaling of this sprite.</param>
        /// <param name="effects">Modificators for drawing. Can be combined.</param>
        /// <param name="layerDepth">A depth of the layer of this sprite.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? sourceRectangle, Color color,
            float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            if (!sourceRectangle.HasValue)
            {
                spriteBatch.Draw(Texture, position, SourceRectangle, color, rotation, origin, scale, effects, layerDepth);
                return;
            }

            spriteBatch.Draw(Texture, position, SourceRectangle.RelativeIntersection(sourceRectangle!.Value), color, rotation, origin, scale, effects, layerDepth);
        }
    }
}
