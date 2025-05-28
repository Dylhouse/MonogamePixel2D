using Microsoft.Xna.Framework;
using MonoGamePixel2D.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Camera
{
    public class RandomLinearShake : IScreenShake
    {
        private float _currentMagnitude;
        /// <summary>
        /// The speed at which the magnitude will change in pixels/sec
        /// </summary>
        private float _magnitudeChangeSpeed;

        private Vector2 _offset;

        private Random _random;

        public Vector2 Offset { get => _offset; }

        public bool IsFinished { get; private set; } = false;

        public RandomLinearShake(float magnitude, TimeSpan duration)
        {
            _currentMagnitude = magnitude;
            _magnitudeChangeSpeed = magnitude / (float)duration.TotalSeconds;
        }

        public void Update(GameTime gameTime)
        {
            _offset = Vector2Extensions.FromPolar(_currentMagnitude, GRandom.random.NextSingle() * MathF.Tau);

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _currentMagnitude -= _magnitudeChangeSpeed * delta;

            if (_currentMagnitude < 0) IsFinished = true;
        }
    }
}
