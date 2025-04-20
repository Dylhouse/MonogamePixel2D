using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePixel2D.Assets.Graphics;
using MonoGamePixel2D.Assets.Graphics.Animations;


namespace MonoGamePixel2D.Assets;

#region Public Methods

/// <summary>
/// Class to easily load and obtain assets.
/// </summary>
public class AssetManager
{
    private const char PrefixDelimiter = '_';
    private const string StaticSpritePrefix = "static";
    private const string AtlasPrefix = "atlas";
    private const string AnimationPrefix = "anim";

    private static readonly JsonSerializerOptions jsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };



    private readonly Dictionary<string, IComplexDrawable> sprites = [];
    private readonly Dictionary<string, AnimatedSprite> animations = [];

    /// <summary>
    /// Initializes a new AssetManager. <b>Make sure to call in <c>LoadContent</c></b>.
    /// </summary>
    /// <param name="content">ContentManager to load content</param>
    /// <param name="dataDir">Directory of JSON or other data</param>
    /// <param name="assetDir">Directory of assets</param>
    public AssetManager(ContentManager content, string dataDir, string assetDir)
    {
        var dataPaths = Directory.GetFiles(Path.Combine(content.RootDirectory, dataDir), "*", SearchOption.AllDirectories)
            .Where(path => Path.GetFileName(path) != null)
            .ToDictionary(path => Path.GetFileName(path)!, path => path);

        var assetPaths = Directory.GetFiles(Path.Combine(content.RootDirectory, assetDir), "*", SearchOption.AllDirectories);

        foreach (var assetPath in assetPaths)
        {
            string assetFileName = Path.GetFileNameWithoutExtension(assetPath);
            if (!IsValidAsset(assetFileName)) continue;

            string assetContentPath = RemoveExtension(Path.GetRelativePath(content.RootDirectory, assetPath));
            string assetType = GetAssetType(assetFileName);
            string assetName = RemoveTypePrefix(assetFileName);

            switch (assetType)
            {
                case AtlasPrefix:

                case StaticSpritePrefix:
                    var sprite = new ImageSprite(content.Load<Texture2D>(assetContentPath));
                    sprites.Add(assetName, sprite);
                    break;

                case AnimationPrefix:
                    try
                    {
                        var animJson = File.ReadAllText(dataPaths[assetFileName + ".json"]);
                        var dto = JsonSerializer.Deserialize<AnimatedSpriteDTO>(animJson, jsonOptions);
                        var texture = content.Load<Texture2D>(assetContentPath);

                        animations.Add(assetName, AnimatedSprite.LoadWithDTO(texture, dto));
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine("AnimatedSprite '" + assetName + "' failed to load! " + e.Message);
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Returns the requested sprite.
    /// </summary>
    /// <param name="name">The name of the sprite.</param>
    /// <returns>The sprite.</returns>
    public IComplexDrawable GetSprite(string name)
    {
        try
        {
            return sprites[name];
        }

        catch (KeyNotFoundException e)
        {
            Console.WriteLine("Sprite name: " + name + " was not found. " + e.Message);
            return null;
        }
    }

    public AnimatedSprite GetAnimation(string name)
    {
        try
        {
            return animations[name];
        }

        catch (KeyNotFoundException e)
        {
            Console.WriteLine("Animation name: " + name + " was not found! " + e.Message);
            return null;
        }
    }
    #endregion
    private static bool IsValidAsset(string fileName)
    {
        var delimeterIndex = fileName.IndexOf(PrefixDelimiter);
        if (delimeterIndex == -1) return false;
        return true;
    }

    /// <summary>
    /// Gets the string before the prefix delimeter (The asset prefix/type).
    /// Returns an empty string if there is none.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>The asset's prefix type (string)</returns>
    private static string GetAssetType(string fileName)
    {
        var delimeterIndex = fileName.IndexOf(PrefixDelimiter);
        bool containsDelimeter = delimeterIndex != -1;

        if (!containsDelimeter) return fileName;

        return fileName[..delimeterIndex];
    }

    private static string RemoveTypePrefix(string name)
    {
        var delimeterIndex = name.IndexOf(PrefixDelimiter);
        bool containsDelimeter = delimeterIndex != -1;

        if (!containsDelimeter) return name;

        return name[(delimeterIndex + 1)..];
    }

    private static string RemoveExtension(string path)
    {
        return path[..path.IndexOf('.')];
    }
}