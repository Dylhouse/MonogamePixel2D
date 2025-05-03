using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePixel2D.Assets.Graphics;

public class ImageSprite(Texture2D texture) : IComplexDrawable, ILoadableAsset
{
    private const string PREFIX = "static";
    /// <inheritdoc>
    public static string Prefix => PREFIX;

    /// <inheritdoc/>
    public static object Load(ContentManager content, string contentPath, string path)
    {
        return new ImageSprite(content.Load<Texture2D>(contentPath));
    }

    #region IComplexDrawable

    /// <inheritdoc/>
    public Texture2D Texture { get; set; } = texture;

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Color color)
    {
        spriteBatch.Draw(Texture, destinationRectangle, color);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
    {
        spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, color);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, color, rotation, origin.ToVector2(), effects, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Point position, Color color)
    {
        spriteBatch.Draw(Texture, position.ToVector2(), color);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color)
    {
        spriteBatch.Draw(Texture, position.ToVector2(), sourceRectangle, color);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(Texture, position.ToVector2(), sourceRectangle, color, rotation,
            origin.ToVector2(), scale, effects, layerDepth);
    }

    /// <inheritdoc/>
    public void Draw(SpriteBatch spriteBatch, Point position, Rectangle? sourceRectangle, Color color,
        float rotation, Point origin, float scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(Texture, position.ToVector2(), sourceRectangle, color, rotation,
            origin.ToVector2(), scale, effects, layerDepth);
    }

    /// <inheritdoc/>
    public void GetData(Color[] data)
    {
        Texture.GetData(data);
    }

    #endregion
}