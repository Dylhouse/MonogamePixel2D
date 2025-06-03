using Microsoft.Xna.Framework;
using MonoGamePixel2D.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.GUI
{
    public abstract class Button : IUpdatable, Assets.Graphics.IDrawable
    {
        static Button()
        {
            InputManager.AddAction("Press Button", new() { MouseButtons = [MouseButtons.LeftButton] });
        }

        public bool ActivateOnRelease { get; set; } = true;

        private bool _hovered = false;
        public bool IsHovered => _hovered;

        protected Rectangle _bounds;
        /// <summary>
        /// Gets the size of the bounds of the button.
        /// </summary>
        public virtual Point Size { get => _bounds.Size; }
        /// <summary>
        /// Gets or sets the location of the button.
        /// </summary>
        public virtual Point Location { get => _bounds.Location; set => _bounds.Location = value; }

        public event Action? Activated;

        public Button(Rectangle bounds)
        {
            _bounds = bounds;
        }

        public void Update(GameTime gameTime)
        {
            _hovered = _bounds.Contains(MouseManager.GetVirtualMousePos());

            // if its hovered, will activate via release if activateOnRelease is true, otherwise on pressed.
            if (_hovered && (ActivateOnRelease ? InputManager.GetActionJustReleased("Press Button") : InputManager.GetActionJustPressed("Press Button")))
            {
                Activated?.Invoke();
            }
        }

        public abstract void Draw(GameTime gameTime);
    }
}
