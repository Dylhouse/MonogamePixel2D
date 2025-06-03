using System;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePixel2D.Assets.Graphics;

/// <summary>
/// A sprite that draws from a specified part of an atlas.
/// </summary>
public class AtlasSprite : IComplexDrawable
{
    /// <summary>
    /// The texture that this sprite will draw from.
    /// </summary>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// The rectangle that this sprite will draw from its <see cref="Texture"/>.
    /// </summary>
    [JsonConverter(typeof(RectangleConverter))]
    public Rectangle SourceRectangle { get; set; }

    /// <summary>
    /// Constructs a new <see cref="AtlasSprite"/>.
    /// </summary>
    /// <param name="texture">The <see cref="Texture2D"/> to be used for the texture atlas.</param>
    /// <param name="sourceRectangle">The rectangle that this sprite will draw from the <paramref name="texture"/>.</param>
    public AtlasSprite(Texture2D texture, Rectangle sourceRectangle)
    {
        Texture = texture;
        SourceRectangle = sourceRectangle;
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color)
    {
        
        spriteBatch.Draw(Texture, destinationRectangle, SourceRectangle, color);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
    {
        
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, destinationRectangle, SourceRectangle, color);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, destinationRectangle, srcRectangle, color);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, SpriteEffects effects, float layerDepth)
    {
        
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, destinationRectangle, SourceRectangle, color, rotation, origin.ToVector2(), effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, destinationRectangle, srcRectangle, color, rotation, origin.ToVector2(), effects, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Point position, Color color)
    {
        
        spriteBatch.Draw(Texture, position.ToVector2(), SourceRectangle, color);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color)
    {
        
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position.ToVector2(), SourceRectangle, color);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, position.ToVector2(), srcRectangle, color);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position.ToVector2(), SourceRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, position.ToVector2(), srcRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, float scale, SpriteEffects effects, float layerDepth)
    {
        
        if (!sourceRectangle.HasValue)
        {
            spriteBatch.Draw(Texture, position.ToVector2(), SourceRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
            return;
        }

        Rectangle srcRectangle = TextureUtils.GetClampedSourceRectangle(SourceRectangle, sourceRectangle.GetValueOrDefault());
        spriteBatch.Draw(Texture, position.ToVector2(), srcRectangle, color, rotation, origin.ToVector2(), scale, effects, layerDepth);
    }

    /// <inheritdoc/>
    public void GetData(Color[] data)
    {
        var size = SourceRectangle.Width * SourceRectangle.Height;
        Texture.GetData(0, SourceRectangle, data, 0, size);
    }
}