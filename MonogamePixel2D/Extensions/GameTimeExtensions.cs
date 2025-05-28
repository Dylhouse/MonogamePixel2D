using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D.Extensions
{
    /// <summary>
    /// Extensions for <see cref="GameTime"/> to easily access useful information about the game's running state.
    /// </summary>
    public static class GameTimeExtensions
    {
        /// <summary>
        /// Calculates the current FPS of the game.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns>The current FPS of the game.</returns>
        public static double GetFPS(this GameTime gameTime) => 1.0d / gameTime.ElapsedGameTime.TotalSeconds;
    }
}
