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
    public int DisplayWidth { get { return _displayWidth; } }

    private readonly int _displayHeight;
    /// <summary>
    /// The height of the user's display.
    /// </summary>
    public int DisplayHeight { get { return _displayHeight; } }

    private readonly int _displayWidth;

    private readonly int _virtualWidth;
    /// <summary>
    /// The width of the game's native resolution.
    /// </summary>
    public int VirtualWidth { get { return _virtualWidth; } }

    private readonly int _virtualHeight;
    /// <summary>
    /// The height of the game's native resolution.
    /// </summary>
    public int VirtualHeight { get { return _virtualHeight; } }

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

    private Rectangle vRect;
    public Rectangle VirtualPort { get => vRect; }

    /// <summary>
    /// Constructs a new instance.
    /// </summary>
    /// <param name="virtualWidth">The width of the game's native resolution.</param>
    /// <param name="virtualHeight">The height of the game's native resolution.</param>
    public GameWindowData(int virtualWidth, int virtualHeight, int displayWidth, int displayHeight)
    {
        this._virtualWidth = virtualWidth;
        this._virtualHeight = virtualHeight;

        vRect = new Rectangle(0, 0, virtualWidth, virtualHeight);

        _displayWidth = displayWidth;
        _displayHeight = displayHeight;

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

    public GameWindowData(int virtualWidth, int virtualHeight) : this(
        virtualWidth,
        virtualHeight,
        GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
        GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
        )
    { }

    public GameWindowData(int virtualWidth, int virtualHeight, int scale)
    {
        this._virtualWidth = virtualWidth;
        this._virtualHeight = virtualHeight;
        _displayWidth = virtualWidth * scale;
        _displayHeight = virtualHeight * scale;

        vRect = new Rectangle(0, 0, virtualWidth, virtualHeight);

        Gameport = new Rectangle(
            0,
            0,
            virtualWidth * scale,
            virtualHeight * scale);
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

        return new(graphicsManager.GraphicsDevice, _virtualWidth, _virtualHeight);
    }
}