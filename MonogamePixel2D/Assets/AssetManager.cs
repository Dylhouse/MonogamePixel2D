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
    /// <summary>
    /// Dictionary with keys being the name of the asset and value being the path to the asset's data, such as a .png.
    /// Additional information, such as JSON files, should be held with the same name at the same directory as sidecar files.
    /// </summary>
    private readonly FrozenDictionary<string, string> _assetPaths;

    private ContentManager _contentManager;

    /// <summary>
    /// Initializes a new AssetManager. <b>Make sure to call in <see cref="Microsoft.Xna.Framework.Game.LoadContent"/></b>.
    /// </summary>
    /// <param name="content">Used to load resources used by assets.</param>
    /// <param name="assetDir">Root directory of all assets relative to <see cref="ContentManager.RootDirectory"/>. Recursively searched to find assets.</param>
    public AssetManager(ContentManager content, string assetDir)
    {
        _contentManager = content;

        var dict = new Dictionary<string, string>();
        foreach (var path in Directory.GetFiles(Path.Combine(content.RootDirectory, assetDir), "*", SearchOption.AllDirectories))
        {
            if (Path.GetExtension(path) == ".json") continue;

            if (!dict.TryAdd(Path.GetFileNameWithoutExtension(path), path)) throw new InvalidOperationException("There cannot be two assets with the same name.");
        }

        _assetPaths = dict.ToFrozenDictionary();
    }

    /// <summary>
    /// Loads resources for an asset if they aren't already loaded and creates an instance of the asset of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="ILoadableAsset"/> to load.</typeparam>
    /// <param name="assetName">The name of the asset to load.</param>
    /// <returns>A new instance of the asset of type <typeparamref name="T"/>.</returns>
    public T LoadAsset<T>(string assetName) where T : ILoadableAsset
    {
        var path = _assetPaths[assetName];
        return (T) T.Load(_contentManager, ContentPathOf(path), path);
    }

    /// <summary>
    /// Unloads resources for an asset.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="ILoadableAsset"/> to unload.</typeparam>
    /// <param name="assetName">The name of the asset to unload.</param>
    public void UnloadAsset<T>(string assetName) where T : ILoadableAsset
    {
        T.Unload(_contentManager, ContentPathOf(_assetPaths[assetName]));
    }

    private string ContentPathOf(string path) =>
        RemoveExtension(Path.GetRelativePath(_contentManager.RootDirectory, path));

    private static string RemoveExtension(string path) =>
        path[..path.IndexOf('.')];
}