using System.Collections.Frozen;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePixel2D.Assets.Graphics;
using MonoGamePixel2D.Assets.Graphics.Animations;


namespace MonoGamePixel2D.Assets;

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


    private readonly FrozenDictionary<string, string> _assetPaths;

    private ContentManager _content;

    /// <summary>
    /// Initializes a new AssetManager. <b>Make sure to call in <see cref="Microsoft.Xna.Framework.Game.LoadContent"/></b>.
    /// </summary>
    /// <param name="content">Used to load content.</param>
    /// <param name="assetDir">Directory of assets and their sidecar files.</param>
    public AssetManager(ContentManager content, string assetDir)
    {
        _content = content;

        var dict = new Dictionary<string, string>();
        foreach (var path in Directory.GetFiles(Path.Combine(content.RootDirectory, assetDir), "*", SearchOption.AllDirectories))
        {
            if (Path.GetExtension(path) == ".json") continue;

            if (!dict.TryAdd(Path.GetFileNameWithoutExtension(path), path)) throw new InvalidOperationException("There cannot be two assets with the same name.");
        }

        _assetPaths = dict.ToFrozenDictionary();


        /*
        _content = content;

        dataPaths = Directory.GetFiles(Path.Combine(content.RootDirectory, dataDir), "*", SearchOption.AllDirectories)
            .Where(path => Path.GetFileName(path) != null)
            .ToDictionary(path => Path.GetFileName(path)!, path => path);

        var assetPaths = Directory.GetFiles(Path.Combine(content.RootDirectory, assetDir), "*", SearchOption.AllDirectories);

        _assetPaths = assetPaths.ToDictionary(path => Path.GetFileNameWithoutExtension(path), path => path);

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
                    break;

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
        */
    }

    /// <summary>
    /// Returns the requested sprite.
    /// </summary>
    /// <param name="name">The name of the sprite.</param>
    /// <returns>The sprite.</returns>
    public IComplexDrawable GetSprite(string name)
    {
        if (_assetPaths.TryGetValue(AtlasPrefix + PrefixDelimiter + name, out var path))
        {
            return null;
        }

        else if (_assetPaths.TryGetValue(StaticSpritePrefix + PrefixDelimiter + name, out path))
        {
            return new ImageSprite(_content.Load<Texture2D>(ContentPathOf(path)));
        }

        else throw new Exception();
    }

    public AnimatedSprite GetAnimatedSprite(string name)
    {
        var path = _assetPaths[AnimationPrefix + PrefixDelimiter + name];

        var animJson = File.ReadAllText(path + ".json");
        var dto = JsonSerializer.Deserialize<AnimatedSpriteDTO>(animJson, jsonOptions);
        var texture = _content.Load<Texture2D>(ContentPathOf(path));

        return AnimatedSprite.LoadWithDTO(texture, dto);
    }

    public T GetAsset<T>(string assetName) where T : ILoadableAsset
    {
        var path = _assetPaths[T.Prefix + PrefixDelimiter + assetName];
        return (T) T.Load(_content, ContentPathOf(path), path);
    }

    /*
    public T GetAsset<T>(string assetName)
    {
        string unprefixedPath = PrefixDelimiter.ToString();
        string prefixlessPath = PrefixDelimiter + Path.Combine(_content.RootDirectory, assetName);

        Type type = typeof(T);

        if (type == typeof(AnimatedSprite))
        {
            var path = _assetPaths[assetName];

            var animJson = File.ReadAllText(path + ".json");
            var dto = JsonSerializer.Deserialize<AnimatedSpriteDTO>(animJson, jsonOptions);
            var texture = _content.Load<Texture2D>(AnimationPrefix + assetName);

            return (T)(object)AnimatedSprite.LoadWithDTO(texture, dto);
        }

        else if (type == typeof(IComplexDrawable))
        {
            var atlasPath = AtlasPrefix + prefixlessPath;
            //implement this, have early return if it is. If not atlas, try static

            var staticPath = StaticSpritePrefix + prefixlessPath;
            return (T)(object)new ImageSprite(_content.Load<Texture2D>(staticPath));
        }

        throw new InvalidOperationException("The asset doesn't exist, or the type is wrong.");
    }
    */


    /*

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
    */

    private static bool IsValidAsset(string fileName) =>
        fileName.Contains(PrefixDelimiter);

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

    private string ContentPathOf(string path) =>
        RemoveExtension(Path.GetRelativePath(_content.RootDirectory, path));

    private static string RemoveExtension(string path) =>
        path[..path.IndexOf('.')];
}