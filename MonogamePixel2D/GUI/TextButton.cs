using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePixel2D.Extensions;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace MonoGamePixel2D.GUI
{
    public class TextButton : Button
    {
        private readonly SpriteBatch _spriteBatch;

        private TextButtonDisplaySettings _settings;

        private Rectangle _insideBounds;

        private string _text;

        /// <summary>
        /// Gets or sets the string value of the displayed text.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                UpdateTextDrawPos();
                
            }
        }

        private Point _textDrawPos;

        public override Point Location { 
            get => base.Location; 
            set
            {
                if (base.Location == value) return;
                base.Location = value;
                _insideBounds.Location = value + new Point(_settings.OutlineWidth, _settings.OutlineWidth);
                UpdateTextDrawPos();
            }
        }

        public TextButton(Rectangle bounds, SpriteBatch spriteBatch, string text, TextButtonDisplaySettings settings) : base(bounds)
        {
            _settings = settings;
            _spriteBatch = spriteBatch;

            var outline = Math.Clamp(settings.OutlineWidth, 0, Math.Min(bounds.Height, bounds.Width) / 2);
            _insideBounds = new Rectangle(bounds.X + outline, bounds.Y + outline, bounds.Width - outline * 2, bounds.Height - outline * 2);
            Text = text;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.DrawRect(_bounds, _settings.OutlineColor);
            _spriteBatch.DrawRect(_insideBounds, _settings.FillColor);

            _spriteBatch.DrawString(_settings.Font, _text, _textDrawPos.ToVector2(), _settings.TextColor);

            if (IsHovered) _spriteBatch.DrawRect(_bounds, _settings.MouseOverOverlayColor);
        }

        private void UpdateTextDrawPos()
        {
            var textBounds = new Point(
                    _text.Length * (_settings.CharacterDimensions.X + _settings.CharacterSpacing) - _settings.CharacterSpacing,
                    _settings.CharacterDimensions.Y
                    );

            _textDrawPos = _insideBounds.Location + PointExtensions.Divide(_insideBounds.Size - textBounds, 2);
        }
    }
}
