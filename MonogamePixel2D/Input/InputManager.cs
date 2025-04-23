using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Input;

/// <summary>
/// Static class for managing inputs, rebinding, etc.
/// </summary>
public static class InputManager
{
    private static readonly TimeSpan _repeatDelay = TimeSpan.FromMilliseconds(250);

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

    private static KeyboardState _currentKeyboardState;
    private static KeyboardState _previousKeyboardState;

    private static MouseState _currentMouseState;
    private static MouseState _previousMouseState;

    private static GamePadState _currentGamepadState;
    private static GamePadState _previousGamepadState;

    public static KeyboardState CurrentKeyboardState => _currentKeyboardState;
    public static KeyboardState PreviousKeyboardState => _previousKeyboardState;


    private readonly static Dictionary<string, InputAction> _actions = [];

    /// <summary>
    /// Adds a new action to the input manager.
    /// </summary>
    /// <param name="name">Name of the action.</param>
    /// <param name="action">The action, containing its associated inputs.</param>
    public static void AddAction(string name, InputAction action) => _actions.Add(name, action);



    #region Get Action

    /// <summary>
    /// Polls an action to see whether or not it was pressed as of the most recent <see cref="Update"/> call.
    /// </summary>
    /// <param name="action">The name of the action to poll.</param>
    /// <returns>True if the action was pressed as of the most recent <see cref="Update"/> call.</returns>
    public static bool GetActionPressed(string action) =>
        _actions[action].Pressed(_currentKeyboardState, _currentGamepadState, _currentMouseState);

    /// <summary>
    /// Polls an action to see whether or not it was released (not pressed) as of the most recent <see cref="Update"/> call.
    /// </summary>
    /// <param name="action">The name of the action to poll.</param>
    /// <returns>True if the action was released (not pressed) as of the most recent <see cref="Update"/> call.</returns>
    public static bool GetActionReleased(string action) => !GetActionPressed(action);

    /// <summary>
    /// Polls an action to see whether or not it was <i>just</i> pressed (going from released to pressed) as of the most recent <see cref="Update"/> call.
    /// </summary>
    /// <param name="action">The name of the action to poll.</param>
    /// <returns>True if the action was <i>just</i> pressed (going from released to pressed) as of the most recent <see cref="Update"/> call.</returns>
    public static bool GetActionJustPressed(string action) =>
        GetActionPressed(action) && _actions[action].Released(_previousKeyboardState, _previousGamepadState, _previousMouseState);

    /// <summary>
    /// Polls an action to see whether or not it was <i>just</i> released (going from pressed to released) as of the most recent <see cref="Update"/> call.
    /// </summary>
    /// <param name="action">The name of the action to poll.</param>
    /// <returns>True if the action was <i>just</i> released (going from pressed to released) as of the most recent <see cref="Update"/> call.</returns>
    public static bool GetActionJustReleased(string action) =>
        GetActionReleased(action) && _actions[action].Pressed(_previousKeyboardState, _previousGamepadState, _previousMouseState);

    #endregion

    #region Get Text Input

    /// <summary>
    /// Gets text input from the keyboard and adds it to <paramref name="currentText"/>.
    /// </summary>
    /// <param name="currentText">The current text; passed in to handle backspace.</param>
    /// <returns><paramref name="currentText"/> with added/removed characters depending on keyboard input.</returns>
    public static string GetTextInput(string currentText = "")
    {
        string output = currentText;
        foreach (var key in _textKeys)
        {
            var dict = _currentKeyboardState.IsKeyDown(Keys.LeftShift) && _previousKeyboardState.IsKeyUp(Keys.RightShift) ? _altKeyChars : _keyChars;

            if (_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key))
            {
                output += dict[key];
            }
        }

        if (output.Length > 0 && _currentKeyboardState.IsKeyDown(Keys.Back) && _previousKeyboardState.IsKeyUp(Keys.Back))
        {
            output = output.Remove(output.Length - 1, 1);
        }

        return output;
    }

    /// <summary>
    /// Adds text input from the keyboard to a <paramref name="stringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">The <see cref="StringBuilder"/> to add the inputted text to.</param>
    public static void AddTextInput(StringBuilder stringBuilder)
    {
        foreach (var key in _textKeys)
        {
            bool useAltKeys = _currentKeyboardState.IsKeyDown(Keys.LeftShift) && _previousKeyboardState.IsKeyUp(Keys.RightShift);
            var dict = _currentKeyboardState.IsKeyDown(Keys.LeftShift) && _previousKeyboardState.IsKeyUp(Keys.RightShift) ? _altKeyChars : _keyChars;

            if (_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key))
            {
                var character = dict[key];
                if (_currentKeyboardState.CapsLock) character = Char.ToUpper(character);

                stringBuilder.Append(character);
            }
        }

        var length = stringBuilder.Length;
        if (length > 0 && _currentKeyboardState.IsKeyDown(Keys.Back) && _previousKeyboardState.IsKeyUp(Keys.Back))
        {
            stringBuilder.Remove(length - 1, 1);
        }
    }

    #endregion

    /// <summary>
    /// Updates the keyboard state to be accurate for the current frame.
    /// </summary>
    public static void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();

        _previousMouseState = _currentMouseState;
        _currentMouseState = Mouse.GetState();
    }
}
