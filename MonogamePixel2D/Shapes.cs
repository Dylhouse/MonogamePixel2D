using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePixel2D.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGamePixel2D
{
    public static class Shapes
    {
        // This isn't my code, its from my goat CraftworksGames on this thread: https://community.monogame.net/t/line-drawing/6962/2

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(Pixel.GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawRect(this SpriteBatch spriteBatch, Point pos, Point size, Color color) =>
            DrawRect(spriteBatch, new Rectangle(pos, size), color);

        public static void DrawRect(this SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            spriteBatch.Draw(Pixel.GetTexture(spriteBatch), rect, color);
        }
    }
}
