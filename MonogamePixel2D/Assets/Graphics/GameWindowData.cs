using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePixel2D.Input;

namespace MonoGamePixel2D.Assets.Graphics;

/// <summary>
/// Contains data about the game and user's display window.
/// </summary>
public class GameWindowData
{
    /// <summary>
    /// The width of the user's display.
    /// </summary>
    public int DisplayWidth { get { return displayWidth; } }

    private readonly int displayHeight;
    /// <summary>
    /// The height of the user's display.
    /// </summary>
    public int DisplayHeight { get { return displayHeight; } }

    private readonly int displayWidth;

    private readonly int virtualWidth;
    /// <summary>
    /// The width of the game's native resolution.
    /// </summary>
    public int VirtualWidth { get { return virtualWidth; } }

    private readonly int virtualHeight;
    /// <summary>
    /// The height of the game's native resolution.
    /// </summary>
    public int VirtualHeight { get { return virtualHeight; } }

    /// <summary>
    /// The integer scale that is applied to the game's native resolution
    /// to make it better fit the user's display.
    /// </summary>
    public int WindowScale { get; }

    /// <summary>
    /// The X offset of the virtual render target on the user's display.
    /// </summary>
    public int XOffset { get { return Gameport.X; } }
    /// <summary>
    /// The Y offset of the virtual render target on the user's display.
    /// </summary>
    public int YOffset { get { return Gameport.Y; } }

    /// <summary>
    /// The rectangle of the upscaled virtual render target that
    /// is drawn to the back buffer.
    /// </summary>
    public Rectangle Gameport { get; }

    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    /// <param name="virtualWidth">The width of the game's native resolution.</param>
    /// <param name="virtualHeight">The height of the game's native resolution.</param>
    public GameWindowData(int virtualWidth, int virtualHeight)
    {
        this.virtualWidth = virtualWidth;
        this.virtualHeight = virtualHeight;

        displayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        displayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        int wScale = displayWidth / virtualWidth;
        int hScale = displayHeight / virtualHeight;
        // Use the lowest possible scale so everything fits
        WindowScale = wScale > hScale ? hScale : wScale;

        Gameport = new Rectangle(
            (displayWidth - virtualWidth * WindowScale) / 2,
            (displayHeight - virtualHeight * WindowScale) / 2,
            virtualWidth * WindowScale,
            virtualHeight * WindowScale);
    }

    /// <summary>
    /// Sets up the <paramref name="graphicsManager"/> for the new display size, and returns the new virtual render target.
    /// </summary>
    /// <param name="graphicsManager">The manager to apply changes to and make the render target with/</param>
    /// <returns>The new virtual render target.</returns>
    public RenderTarget2D SetupGameWindow(GraphicsDeviceManager graphicsManager)
    {
        MouseManager.SetWindowData(this);

        graphicsManager.PreferredBackBufferWidth = DisplayWidth;
        graphicsManager.PreferredBackBufferHeight = DisplayHeight;
        graphicsManager.ApplyChanges();

        return new(graphicsManager.GraphicsDevice, virtualWidth, virtualHeight);
    }
}