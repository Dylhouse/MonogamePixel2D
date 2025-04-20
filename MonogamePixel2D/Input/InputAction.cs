using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Input
{
    /// <summary>
    /// Represents a game input action that can be activated by an arrangement of keyboard keys,
    /// buttons, or mouse buttons.
    /// </summary>
    public struct InputAction
    {
        /// <summary>
        /// The keys that are polled for input.
        /// </summary>
        public Keys[]? Keys;

        /// <summary>
        /// The buttons that are polled for input.
        /// </summary>
        public Buttons[]? Buttons;

        /// <summary>
        /// The mouse buttons that are polled for input.
        /// </summary>
        public MouseButtons[]? MouseButtons;

        internal readonly bool Pressed(KeyboardState keyState, GamePadState padState, MouseState mouseState)
        {
            if (Keys != null)
            {
                foreach (Keys key in Keys)
                {
                    if (keyState.IsKeyDown(key)) return true;
                }
            }
            if (Buttons != null)
            {
                throw new NotImplementedException("Gamepad support isn't implemented yet, go tell Liam to do it.");
            }
            if (MouseButtons != null)
            {
                foreach (MouseButtons button in MouseButtons)
                {
                    if (GetMouseButtonState(mouseState, button) == ButtonState.Pressed) return true;
                }
            }
            return false;
        }

        internal readonly bool Released(KeyboardState keyState, GamePadState padState, MouseState mouseState) =>
            !Pressed(keyState, padState, mouseState);

        private static ButtonState GetMouseButtonState(MouseState state, MouseButtons button)
        {
            switch (button)
            {
                case Input.MouseButtons.LeftButton:
                    return state.LeftButton;

                case Input.MouseButtons.MiddleButton:
                    return state.MiddleButton;

                case Input.MouseButtons.RightButton:
                    return state.RightButton;

                case Input.MouseButtons.Button4:
                    return state.XButton1;

                case Input.MouseButtons.Button5:
                    return state.XButton2;

                default:
                    throw new Exception("A mouse button that does not exist was passed in.");
            }
        }
    }
}
