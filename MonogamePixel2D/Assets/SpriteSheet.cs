using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Assets
{
    public class SpriteSheet : ISpriteSheetAsset, ILoadableAsset
    {
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            Converters = { new RectangleConverter() }
        };

        private readonly Texture2D _texture;
        private readonly Dictionary<string, Rectangle> _sourceRectangles;
        private readonly string _path;

        private readonly Point _offset;

        private SpriteSheet(Texture2D texture, Rectangle sourceRectangle, string path)
        {
            _texture = texture;
            _offset = sourceRectangle.Location;
            _path = path;
            // trust the null bro 💀
            _sourceRectangles = JsonSerializer.Deserialize<Dictionary<string, Rectangle>>
                (File.ReadAllText(Path.ChangeExtension(path, ".json")), jsonOptions)!;

        }

        public static object Load(Texture2D sheetTexture, Rectangle sourceRectangle, string path) =>
            new SpriteSheet(sheetTexture, sourceRectangle, path);

        public static object Load(ContentManager content, string contentPath, string path) =>
            new SpriteSheet(content.Load<Texture2D>(contentPath), Rectangle.Empty, path);

        public static void Unload(ContentManager content, string contentPath) =>
            content.UnloadAsset(contentPath);

        public T Get<T>(string assetName) where T : ISpriteSheetAsset
        {
            var sourceRectangle = _sourceRectangles[assetName];
            sourceRectangle.Offset(_offset);
            return (T)T.Load(_texture, sourceRectangle, _path);
        }
    }
}
