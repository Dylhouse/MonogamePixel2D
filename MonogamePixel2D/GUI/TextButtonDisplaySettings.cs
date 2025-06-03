

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePixel2D.GUI
{
    /// <summary>
    /// Settings for the text button class, including colors, outline, etc.
    /// </summary>
    public class TextButtonDisplaySettings
    {
        /// <summary>
        /// The font to be used by the button. Must be <u>monospaced</u>.
        /// </summary>
        public required SpriteFont Font { get; set; }

        /// <summary>
        /// The dimensions of each character of the monospaced <see cref="Font"/>
        /// </summary>
        public required Point CharacterDimensions { get; set; }

        /// <summary>
        /// The spacing between characters of the monospaced <see cref="Font"/>.
        /// </summary>
        public required int CharacterSpacing { get; set; }

        /// <summary>
        /// The color to be used for the button's outline.
        /// </summary>
        public Color OutlineColor { get; set; } = Color.Gray;
        /// <summary>
        /// The color to be used for the button's interior (not outline).
        /// </summary>
        public Color FillColor { get; set; } = Color.White;
        /// <summary>
        /// The color to be used for the text of the button.
        /// </summary>
        public Color TextColor { get; set; } = Color.DarkGray;

        /// <summary>
        /// The color for the rectangle that will be overlayed on the button when the mouse is over it. It is recommended to give
        /// this a significant amount of alpha, unless you want your button to turn into this color when the mouse is dragged over it.s
        /// </summary>
        public Color MouseOverOverlayColor { get; set; } = new Color(128, 128, 128, 128);

        /// <summary>
        /// The width of the button's outline.
        /// </summary>
        public int OutlineWidth { get; set; } = 1;
    }
}
