using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGamePixel2D.Collisions;

namespace MonoGamePixel2D.Camera
{
    /// <summary>
    /// Class for holding data about a camera in a <see cref="Matrix"/>.
    /// </summary>
    public class Camera : IUpdatable, IPositioned
    {
        private List<IScreenShake> _appliedShakes;

        private Matrix _matrix = Matrix.Identity;

        /// <summary>
        /// The offset of the camera to the <see cref="Position"/>. This can be used to put the player in the middle of the screen,
        /// for example.
        /// </summary>
        public Point Offset { get; set; } = Point.Zero;

        /// <summary>
        /// Gets the transform matrix of the camera.
        /// </summary>
        public Matrix TransformMatrix { get => _matrix; }

        /// <inheritdoc/>
        public Vector2 Position
        {
            get => new(_matrix.M41, _matrix.M42);
            set
            {
                _matrix.M41 = value.X;
                _matrix.M42 = value.Y;
            }
        }

        /// <summary>
        /// The function used to determine the camera's position.
        /// </summary>
        public Func<Vector2> PositionProvider { get; set; }

        /// <summary>
        /// Constructs a new <see cref="Camera"/> with a target.
        /// </summary>
        /// <param name="positionProvider">The function the camera will use to determine its position.</param>
        public Camera(Func<Vector2> positionProvider)
        {
            _appliedShakes = [];
            PositionProvider = positionProvider;
        }

        /// <summary>
        /// Updates the camera's position to match the <see cref="Target"/>. Make sure to call <b>after</b> you update
        /// the <see cref="Target"/>'s position so the camera doesn't jitter.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            var newVect = ((-PositionProvider()).ToPoint() + Offset).ToVector2();

            foreach (var shake in _appliedShakes)
            {
                shake.Update(gameTime);
                newVect += shake.Offset;
            }

            _appliedShakes.RemoveAll(shake => shake.IsFinished);
            Position = newVect;
        }

        public void AddShake(IScreenShake shake)
        {
            _appliedShakes.Add(shake);
        }
    }
}
