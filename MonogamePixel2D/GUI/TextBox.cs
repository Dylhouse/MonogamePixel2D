using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGamePixel2D.Assets.Graphics;
using MonoGamePixel2D.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDrawable = MonoGamePixel2D.Assets.Graphics.IDrawable;

namespace MonoGamePixel2D.GUI
{
    public class TextBox : IUpdatable, IDrawable
    {
        private static readonly TimeSpan CursorBlinkSpeed = TimeSpan.FromMilliseconds(500.0d);

        private static readonly Point CursorOffset = new(0, 2);

        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;
        private readonly TextInput _textInputData = new();

        private bool _cursorVisible = false;
        private Timer _cursorBlinkTimer;

        public TextInput TextInput => _textInputData;

        public Point DrawPos { get; set; }

        public Color TextColor { get; set; }
        public Color CursorColor { get; set; }

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;

                if (!value)
                {
                    _cursorBlinkTimer.Stop();
                    _cursorBlinkTimer.Reset();
                }
            }
        }
        private bool _active = false;

        public TextBox(SpriteFont font, SpriteBatch spriteBatch, Point drawPos, Color textColor, Color cursorColor)
        {
            _cursorBlinkTimer = new(CursorBlinkSpeed, Timer.LoopMode.ContinousLoop);
            _cursorBlinkTimer.Finished += OnBlinkTimerFinished;

            _textInputData.Changed += OnTextInputChanged;

            _font = font;
            _spriteBatch = spriteBatch;
            DrawPos = drawPos;

            TextColor = textColor;
            CursorColor = cursorColor;
        }
        public TextBox(SpriteFont font, SpriteBatch spriteBatch, Point drawPos) : this(font, spriteBatch, drawPos, Color.White, Color.Aqua) { }

        public void Update(GameTime gameTime)
        {
            if (!Active) return;

            _textInputData.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.DrawString(_font, _textInputData.ToString(), DrawPos.ToVector2(), TextColor);

            if (Active)
            {
                _cursorBlinkTimer.Update(gameTime);

                if (_cursorVisible)
                {
                    var cursorStr = new string(' ', _textInputData.CursorIndex) + '_' + new string(' ', _textInputData.Text.Length - _textInputData.CursorIndex);
                    _spriteBatch.DrawString(_font, cursorStr, (DrawPos + CursorOffset).ToVector2(), CursorColor);
                }
            }
        }

        private void OnTextInputChanged()
        {
            _cursorVisible = true;
            _cursorBlinkTimer.Reset();
        }

        private void OnBlinkTimerFinished()
        {
            _cursorVisible = !_cursorVisible;
        }
    }
}
