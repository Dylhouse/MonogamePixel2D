using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace MonoGamePixel2D.Input;

/// <summary>
/// Static class for managing inputs, rebinding, etc.
/// </summary>
public static class InputManager
{
    private static KeyboardState _currentKeyboardState;
    private static KeyboardState _previousKeyboardState;

    private static MouseState _currentMouseState;
    private static MouseState _previousMouseState;

    private static GamePadState _currentGamepadState;
    private static GamePadState _previousGamepadState;

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
