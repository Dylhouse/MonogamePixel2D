using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D;

/// <summary>
/// Static class for managing inputs, rebinding, etc.
/// </summary>
public static class InputManager
{
    private static KeyboardState _currentKeyboardState;
    private static KeyboardState _previousKeyboardState;
    private static Dictionary<string, Keys[]> _inputKeys = [];

    /// <summary>
    /// Adds a new input to the input manager.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="keys"></param>
    public static void AddKeyInput(string name, params Keys[] keys)
    {
        _inputKeys.Add(name, keys);
    }

    #region Get Input    

    /// <summary>
    /// ADD ERROR HANDLING!!!
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool GetInputPressed(string input)
    {
        foreach (Keys key in _inputKeys[input])
        {
            if (_currentKeyboardState.IsKeyDown(key)) return true;
        }
        return false;
    }

    /// <summary>
    /// ADD ERROR HANDLING!!!
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool GetInputReleased(string input)
    {
        return !GetInputPressed(input);
    }

    /// <summary>
    /// ADD ERROR HANDLING!!!
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool GetInputJustPressed(string input)
    {
        foreach (Keys key in _inputKeys[input])
        {
            if (_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key)) return true;
        }
        return false;
    }

    /// <summary>
    /// ADD ERROR HANDLING!!!
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool GetInputJustReleased(string input)
    {
        foreach (Keys key in _inputKeys[input])
        {
            if (_currentKeyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key)) return true;
        }
        return false;
    }

    #endregion

    /// <summary>
    /// Updates the keyboard state to be accurate for the current frame.
    /// </summary>
    public static void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();
    }
}
