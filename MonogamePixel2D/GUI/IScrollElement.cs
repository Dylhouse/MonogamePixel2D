using MonoGamePixel2D.Assets.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.GUI
{
    public interface IScrollElement : IDrawable, IUpdatable, ILocated
    {
        public int Height { get; }
    }
}
