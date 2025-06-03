using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Camera
{
    public interface IScreenShake : IUpdatable
    {
        public Vector2 Offset { get; }

        public bool IsFinished { get; }
    }
}
