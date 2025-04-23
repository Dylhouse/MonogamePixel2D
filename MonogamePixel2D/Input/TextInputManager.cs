using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace MonoGamePixel2D.Input
{
    public static class TextInputManager
    {
        private static readonly TimeSpan RepeatDelay = TimeSpan.FromMilliseconds(250.0d);
        /// <summary>
        /// Default windows repeat rate of 30 keystrokes/sec (1/30 sec/keystroke).
        /// </summary>
        private static readonly TimeSpan RepeatRate = TimeSpan.FromSeconds(1.0d / 30.0d);

        private static TimeSpan _holdingStartTime;
        private static TimeSpan _repeatTime = TimeSpan.Zero;

        private static Keys _heldKey;

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
            { Keys.OemComma, '<' }
        }.ToFrozenDictionary();

        private readonly static ImmutableArray<Keys> _textKeys = _keyChars.Keys;
        #endregion

       
        /// <summary>
        /// Adds text input from the keyboard to a <paramref name="stringBuilder"/>.
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/> to add the inputted text to.</param>
        public static void AppendTextInput(StringBuilder stringBuilder, GameTime gameTime)
        {
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
                            while (_repeatTime > RepeatRate)
                            {
                                stringBuilder.Append(KeyToChar(key, useAltKeys, capsLock));
                                _repeatTime -= RepeatRate;
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

                    stringBuilder.Append(KeyToChar(key, useAltKeys, capsLock));
                }
            }

            if (keyJustPressed) _holdingStartTime = gameTime.TotalGameTime;

            var length = stringBuilder.Length;

            if (length > 0 && currentState.IsKeyDown(Keys.Back) && previousState.IsKeyUp(Keys.Back))
            {
                stringBuilder.Remove(length - 1, 1);
            }
        }

        private static char KeyToChar(Keys key, bool shifted, bool capsLock)
        {
            var dict = shifted ? _altKeyChars : _keyChars;

            var character = dict[key];
            if (capsLock) character = Char.ToUpper(character);
            
            return character;
        }
    }
}
