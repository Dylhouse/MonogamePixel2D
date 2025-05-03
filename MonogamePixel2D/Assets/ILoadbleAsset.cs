using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePixel2D.Assets
{
    /// <summary>
    /// Represents an asset that can be loaded through <see cref="AssetManager.GetAsset{T}(string)"/> and placed in the asset folder
    /// hierarchy (with its prefix; {prefix}_{assetName}).
    /// </summary>
    public interface ILoadableAsset
    {
        /// <summary>
        /// The prefix for the asset that should be used in the asset folder hierarchy so it can be found by <see cref="AssetManager"/>.
        /// </summary>
        public abstract static string Prefix { get; }

        /// <summary>
        /// Loads the asset.
        /// </summary>
        /// <param name="content">Used to load any .xnb files for the asset, such as a <see cref="Texture2D"/>.</param>
        /// <param name="contentPath">The path to be passed into <see cref="ContentManager.Load{T}(string)"/>.</param>
        /// <param name="path">The absolute path of the asset, to find sidecar files such as JSONs.</param>
        /// <returns>The asset as an <see cref="object"/>.</returns>
        public abstract static object Load(ContentManager content, string contentPath, string path);
    }
}
