using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGamePixel2D.Assets.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Input
{
    /// <summary>
    /// Static class for managing mouse input. Methods involving the window/virtual target necessitate
    /// <see cref="SetWindowData(GameWindowData)"/> to be called.
    /// </summary>
    public static class MouseManager
    {
        private static MouseState _currentMouseState;
        private static MouseState _previousMouseState;

        /// <summary>
        /// Returns the current <c>MouseState</c> from when <c>Update()</c> was called.
        /// </summary>
        public static MouseState MouseState => _currentMouseState;

        /// <summary>
        /// Returns the <c>MouseState</c> from when <c>Update()</c> was previously called.
        /// </summary>
        public static MouseState PreviousMouseState => _previousMouseState;

        /// <summary>
        /// Gets the change in scroll wheel value between the most recent and previous <see cref="Update"/> calls.
        /// </summary>
        /// <returns>The change in scroll wheel value. As velocity, it would be value/ticks.</returns>
        public static int ScrollWheelChange =>
            _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;

        /// <summary>
        /// Gets the change in horizontal scroll wheel value between the most recent and previous <see cref="Update"/> calls.
        /// </summary>
        /// <returns>The change in horizontal scroll wheel value. As velocity, it would be value/ticks.</returns>
        public static int HorizontalScrollWheelChange =>
            _currentMouseState.HorizontalScrollWheelValue - _previousMouseState.HorizontalScrollWheelValue;

        private static GameWindowData? _windowData;

        /// <summary>
        /// Sets the <see cref="GameWindowData"/> to be used for methods involving the window/virtual target.
        /// </summary>
        /// <param name="gameWindowData">The <see cref="GameWindowData"/> to be used.</param>
        public static void SetWindowData(GameWindowData gameWindowData)
        {
            _windowData = gameWindowData;
        }

        /// <summary>
        /// Gets the mouse's position in the window, disregarding the virtual render target or other factors.
        /// </summary>
        /// <returns>The mouse's absolute position.</returns>
        public static Point GetAbsoluteMousePos() => _currentMouseState.Position;

        /// <summary>
        /// Gets the mouse's position scaled and relative to the virtual render target.
        /// </summary>
        /// <returns>The virtual mouse pos.</returns>
        public static Point GetVirtualMousePos()
        {
            Debug.Assert(_windowData != null);
            int mouseX = (_currentMouseState.X + _windowData.XOffset) / _windowData.WindowScale;
            int mouseY = (_currentMouseState.Y - _windowData.YOffset) / _windowData.WindowScale;
            return new Point(mouseX, mouseY);
        }

        /// <summary>
        /// Gets the mouse's position scaled and relative to the virtual render target, with the position clamped
        /// to remain within its bounds.
        /// </summary>
        /// <returns>The virtual mouse pos clamped to the virtual render target's bounds.</returns>
        public static Point GetVirtualMousePosClamped()
        {
            Debug.Assert(_windowData != null);
            var pos = GetVirtualMousePos();
            pos.X = Math.Clamp(pos.X, 0, _windowData.VirtualWidth);
            pos.Y = Math.Clamp(pos.Y, 0, _windowData.VirtualHeight);
            return pos;
        }

        /// <summary>
        /// Updates the <c>MouseState</c>s that are used.
        /// </summary>
        public static void Update()
        {
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
        }
    }
}
