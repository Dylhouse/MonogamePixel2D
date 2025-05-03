using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text;

namespace MonoGamePixel2D.Input
{
    public class TextInput : IUpdatable
    {
        #region Input

        #region Key Characters
        private readonly static FrozenDictionary<Keys, char> _keyChars = new Dictionary<Keys, char>()
        {
            { Keys.A, 'a' },
            { Keys.B, 'b' },
            { Keys.C, 'c' },
            { Keys.D, 'd' },
            { Keys.E, 'e' },
            { Keys.F, 'f' },
            { Keys.G, 'g' },
            { Keys.H, 'h' },
            { Keys.I, 'i' },
            { Keys.J, 'j' },
            { Keys.K, 'k' },
            { Keys.L, 'l' },
            { Keys.M, 'm' },
            { Keys.N, 'n' },
            { Keys.O, 'o' },
            { Keys.P, 'p' },
            { Keys.Q, 'q' },
            { Keys.R, 'r' },
            { Keys.S, 's' },
            { Keys.T, 't' },
            { Keys.U, 'u' },
            { Keys.V, 'v' },
            { Keys.W, 'w' },
            { Keys.X, 'x' },
            { Keys.Y, 'y' },
            { Keys.Z, 'z' },
            { Keys.D0, '0' },
            { Keys.D1, '1' },
            { Keys.D2, '2' },
            { Keys.D3, '3' },
            { Keys.D4, '4' },
            { Keys.D5, '5' },
            { Keys.D6, '6' },
            { Keys.D7, '7' },
            { Keys.D8, '8' },
            { Keys.D9, '9' },
            { Keys.Space, ' ' },
            { Keys.OemTilde, '`' },
            { Keys.OemSemicolon, ';' },
            { Keys.OemQuotes, '\\' },
            { Keys.OemQuestion, '/' },
            { Keys.OemPlus, '=' },
            { Keys.OemPipe, '\\' },
            { Keys.OemPeriod, '.' },
            { Keys.OemOpenBrackets, '[' },
            { Keys.OemCloseBrackets, ']' },
            { Keys.OemMinus, '-' },
            { Keys.OemComma, ',' },
            // should never need to get the value of this bc special key, so min value it is for now
            { Keys.Back, Char.MinValue },
            { Keys.Left, Char.MinValue },
            { Keys.Right, Char.MinValue }
        }.ToFrozenDictionary();

        private readonly static FrozenDictionary<Keys, char> _altKeyChars = new Dictionary<Keys, char>()
        {
            { Keys.A, 'A' },
            { Keys.B, 'B' },
            { Keys.C, 'C' },
            { Keys.D, 'D' },
            { Keys.E, 'E' },
            { Keys.F, 'F' },
            { Keys.G, 'G' },
            { Keys.H, 'H' },
            { Keys.I, 'I' },
            { Keys.J, 'J' },
            { Keys.K, 'K' },
            { Keys.L, 'L' },
            { Keys.M, 'M' },
            { Keys.N, 'N' },
            { Keys.O, 'O' },
            { Keys.P, 'P' },
            { Keys.Q, 'Q' },
            { Keys.R, 'R' },
            { Keys.S, 'S' },
            { Keys.T, 'T' },
            { Keys.U, 'U' },
            { Keys.V, 'V' },
            { Keys.W, 'W' },
            { Keys.X, 'X' },
            { Keys.Y, 'Y' },
            { Keys.Z, 'Z' },
            { Keys.D0, ')' },
            { Keys.D1, '!' },
            { Keys.D2, '@' },
            { Keys.D3, '#' },
            { Keys.D4, '$' },
            { Keys.D5, '%' },
            { Keys.D6, '^' },
            { Keys.D7, '&' },
            { Keys.D8, '*' },
            { Keys.D9, '(' },
            { Keys.Space, ' ' },
            { Keys.OemTilde, '~' },
            { Keys.OemSemicolon, ':' },
            { Keys.OemQuotes, '"' },
            { Keys.OemQuestion, '?' },
            { Keys.OemPlus, '+' },
            { Keys.OemPipe, '|' },
            { Keys.OemPeriod, '>' },
            { Keys.OemOpenBrackets, '{' },
            { Keys.OemCloseBrackets, '}' },
            { Keys.OemMinus, '_' },
            { Keys.OemComma, '<' },
            // should never need to get the value of this bc special key, so min value it is for now
            { Keys.Back, Char.MinValue },
            { Keys.Left, Char.MinValue },
            { Keys.Right, Char.MinValue }
        }.ToFrozenDictionary();

        private readonly static ImmutableArray<Keys> _textKeys = _keyChars.Keys;
        #endregion

        private static TimeSpan RepeatDelay = TimeSpan.FromMilliseconds(333.3d);

        /// <summary>
        /// Default: windows repeat rate of 30 keystrokes/sec (1/30 sec/keystroke).
        /// </summary>
        private static TimeSpan RepeatPeriod = TimeSpan.FromSeconds(1.0d / 30.0d);

        private TimeSpan _holdingStartTime;
        private TimeSpan _repeatTime = TimeSpan.Zero;

        private Keys _heldKey;

        /// <summary>
        /// Invoked if the text or cursor position was changed during an <see cref="Update(GameTime)"/> call.
        /// </summary>
        public event Action? Changed;

        /// <summary>
        /// Updates this with any keboard input from the most recent <see cref="InputManager"/> update cycle.
        /// </summary>
        /// <param name="gameTime">Used to calculate when a character should start repeating and how fast it repeats.</param>
        public void Update(GameTime gameTime)
        {
            var previousCursorIndex = _cursorIndex;
            var previousString = _stringBuilder.ToString();

            var currentState = InputManager.CurrentKeyboardState;
            var previousState = InputManager.PreviousKeyboardState;

            bool keyJustPressed = false;

            foreach (var key in _textKeys)
            {
                bool useAltKeys = currentState.IsKeyDown(Keys.LeftShift) || currentState.IsKeyDown(Keys.RightShift);
                bool capsLock = currentState.CapsLock;

                // handle holding a key to make it go a bunch of times
                if (_heldKey == key)
                {
                    if (currentState.IsKeyDown(key))
                    {
                        if (_holdingStartTime + RepeatDelay < gameTime.TotalGameTime)
                        {
                            _repeatTime += gameTime.ElapsedGameTime;
                            while (_repeatTime > RepeatPeriod)
                            {
                                HandleKeyInput(key, useAltKeys, capsLock);
                                _repeatTime -= RepeatPeriod;
                            }
                        }
                    }

                    else
                    {
                        _heldKey = Keys.None;
                    }
                }

                // if this is true then the key was just pressed
                else if (currentState.IsKeyDown(key) && previousState.IsKeyUp(key))
                {
                    keyJustPressed = true;
                    _heldKey = key;

                    HandleKeyInput(key, useAltKeys, capsLock);
                }
            }

            if (keyJustPressed) _holdingStartTime = gameTime.TotalGameTime;

            if (!IsChanged(previousCursorIndex, previousString)) Changed?.Invoke();
        }

        private bool IsChanged(int previousCursorIndex, string previousString)
        {
            return _cursorIndex == previousCursorIndex && _stringBuilder.ToString() == previousString;
        }

        private void HandleKeyInput(Keys key, bool shifted, bool capsLock)
        {
            switch (key)
            {
                case Keys.Back:
                    Backspace(); break;

                case Keys.Left:
                    MoveCursor(-1); break;

                case Keys.Right:
                    MoveCursor(1); break;

                default:
                    InsertChar(KeyToChar(key, shifted, capsLock)); break;
            }
        }

        private static char KeyToChar(Keys key, bool shifted, bool capsLock)
        {
            var dict = shifted ? _altKeyChars : _keyChars;

            var character = dict[key];
            if (capsLock) character = Char.ToUpper(character);

            return character;
        }

        #endregion

        public StringBuilder Text => _stringBuilder;
        private readonly StringBuilder _stringBuilder = new();

        private int _cursorIndex = 0;

        /// <summary>
        /// The index of the text cursor.
        /// </summary>
        public int CursorIndex
        {
            get => _cursorIndex;
            set
            {
                if (_cursorIndex == value) return;
                
                if (value < 0) _cursorIndex = 0;
                else if (value > _stringBuilder.Length) _cursorIndex = value;
            }
        }

        /// <summary>
        /// Clears the text.
        /// </summary>
        public void Clear()
        {
            _stringBuilder.Clear();
            _cursorIndex = 0;
        }

        public void Backspace()
        {
            if (_cursorIndex == 0) return;
            _stringBuilder.Remove(_cursorIndex-- - 1, 1);
        }

        public void MoveCursor(int dir) =>
            _cursorIndex = Math.Clamp(_cursorIndex + dir, 0, _stringBuilder.Length);

        public void InsertChar(char @char)
        {
            _stringBuilder.Insert(_cursorIndex++, @char);
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
