using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D
{
    public static class GRandom
    {
        public static readonly Random random;
        static GRandom()
        {
            random = new Random();
        }
    }
}
