using Microsoft.Xna.Framework;
using MonoGamePixel2D.Input;
namespace MonoGamePixel2D.GUI
{
    public class ScrollBox : Assets.Graphics.IDrawable, IUpdatable
    {
        private int _numToUpdate;
        public bool AllowCutoff { get; set; } = true;

        public readonly List<IScrollElement> elements = [];

        private readonly Rectangle _boxArea;

        private readonly int _lowestY;

        private readonly Point startPosition;
        private Point currentPosition;

        public int ElementSpacing { get; set; } = 5;

        public ScrollBox(Rectangle area)
        {
            _boxArea = area;
            startPosition = area.Location;
            currentPosition = startPosition;
        }
        public void Draw(GameTime gameTime)
        {
            var scrollOffset = -MouseManager.ScrollWheelChange;
            scrollOffset /= 3;
            currentPosition.Y += scrollOffset;

            int highestY = startPosition.Y;
            foreach (var element in elements)
            {
                highestY -= element.Height + ElementSpacing;
            }
            highestY += ElementSpacing;

            currentPosition.Y = Math.Clamp(currentPosition.Y, highestY, startPosition.Y);

            Point drawPos = currentPosition;

            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (drawPos.Y < startPosition.Y)
                {
                    if (!AllowCutoff) goto NextElement;
                    if (drawPos.Y + element.Height < startPosition.Y) goto NextElement;
                }

                element.Location = drawPos;
                element.Draw(gameTime);

            NextElement:
                drawPos.Y += element.Height + ElementSpacing;
            }

            // all this code gets the lowest (but highest numerically) position of the elements based on how many there are to see how far we can scroll



            /*int lowestDrawY = startPosition.Y;
            foreach (var element in elements)
            {
                lowestDrawY -= element.Height + ElementSpacing;
            }
            lowestDrawY += ElementSpacing;

            if (lowestDrawY < _boxArea.Bottom) lowestDrawY = _boxArea.Bottom;

            currentPosition.Y = Math.Clamp(currentPosition.Y, startPosition.Y, lowestDrawY);

            var drawPos = currentPosition;
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                
                bool willCutoff = drawPos.Y > _boxArea.Bottom + element.Height;
                if (willCutoff && !AllowCutoff) return;

                element.Location = drawPos;
                element.Draw(gameTime);
                _numToUpdate = i;

                if (willCutoff) return;

                drawPos.Y += element.Height + ElementSpacing;
            }
            */
        }

        public void Clear()
        {
            elements.Clear();
            currentPosition = Point.Zero;
        }

        public void Update(GameTime gameTime)
        {
            int count = elements.Count;
            if (count == 0) return;
            for (int i = 0; i < count; i++)
            {
                elements[i].Update(gameTime);
            }
        }
    }
}
