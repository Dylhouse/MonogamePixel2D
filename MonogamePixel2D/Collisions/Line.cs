using Microsoft.Xna.Framework;
using MonoGamePixel2D;
namespace MonoGamePixel2D.Collisions
{
    public struct Line(float x1, float y1, float x2, float y2)
    {
        public float X1 = x1;
        public float Y1 = y1;
        public float X2 = x2;
        public float Y2 = y2;

        public readonly Vector2 Start => new(X1, Y1);
        public readonly Vector2 End => new(X2, Y2);

        public readonly float Slope => (Y2 - Y1) / (X2 - X1);

        /// <summary>
        /// Gets the length of the <see cref="Line"/>.
        /// It's recommended to use <see cref="LengthSquared"/> instead of <see cref="Length"/> for comparisons as <see cref="Length"/>
        /// is a marginally slower calculation because of the use of <see cref="MathF.Sqrt(float)"/>.
        /// </summary>
        public readonly float Length => MathF.Sqrt((Y2 - Y1) * (Y2 - Y1) + (X2 - X1) * (X2 - X1));

        /// <summary>
        /// Gets the squared length of the <see cref="Line"/>. Marginally faster than <see cref="Length"/>.
        /// </summary>
        public readonly float LengthSquared => (Y2 - Y1) * (Y2 - Y1) + (X2 - X1) * (X2 - X1);

        /// <summary>
        /// Gets the angle of the <see cref="Line"/> in radians.
        /// </summary>
        public readonly float Angle => MathF.Atan2(Y2 - Y1, X2 - X1);

        /// <summary>
        /// Gets the vector representing the <see cref="Line"/>'s length and direction. Position is lost.
        /// </summary>
        public readonly Vector2 Vector => new(X2 - X1, Y2 - Y1);

        public Line(Vector2 start, Vector2 end) : this(start.X, start.Y, end.X, end.Y) { }
        public Line(Vector4 lineVec) : this(lineVec.X, lineVec.Y, lineVec.Z, lineVec.W) { }

        /// <summary>
        /// Gets 
        /// </summary>
        /// <param name="cellSize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public readonly Point[] GetIntersectingCellIndices(int cellSize, int offset)
        {
            // ensures that x1 < x2, a tad bit stupid but honestly idc
            float X1;
            float X2;
            float Y1;
            float Y2;
            if (this.X1 > this.X2)
            {
                X1 = this.X2;
                Y1 = this.Y2;
                X2 = this.X1;
                Y2 = this.Y1;
            }
            else
            {
                X1 = this.X1;
                Y1 = this.Y1;
                X2 = this.X2;
                Y2 = this.Y2;
            }

            float xLen = (X2 - X1);
            float yDisplacement = (Y2 - Y1);

            int yDir = Y1 < Y2 ? 1 : -1;

            int xIntercepts = (int)X2 / cellSize - (int)X1 / cellSize;
            int yIntercepts = Math.Abs(((int)Y2 / cellSize) * yDir - (int)Y1 / cellSize);

            Point[] cells = new Point[xIntercepts + yIntercepts + 1];

            //cells[0] = new Point((int)MathF.Floor(X1) / cellSize * cellSize, (int)MathF.Floor(Y1) / cellSize * cellSize);

            int cellX1 = (int)MathF.Floor(X1 / cellSize);
            int cellY1 = (int)MathF.Floor(Y1 / cellSize);

            //int cellX1 = (int)MathF.Floor(X1) / cellSize;
            //int cellY1 = (int)MathF.Floor(Y1) / cellSize;

            // fills in 1st cell
            //cells[0] = new Point(cellX1, cellY1);

            int cellIndex = 0;

            int yOffset = 0;
            for (int xOffset = 0; xOffset <= xIntercepts; xOffset++)
            {
                float xAtInt = MathF.BitDecrement(((int)(X1 + (float)cellSize) / cellSize * cellSize) + cellSize * xOffset);
                if (xAtInt > X2)
                {
                    //now its the last one so we set it to xLen
                    xAtInt = xLen;
                }
                else xAtInt -= X1;

                float distRatio = xAtInt / xLen;

                float nextY = yDisplacement * distRatio + Y1;
                int yInts = Math.Abs(((int)nextY / cellSize) - ((int)Y1 / cellSize) - yOffset);

                cells[cellIndex] = new Point(cellX1 + xOffset, cellY1 + yOffset);
                for (int j = 1; j <= yInts; j++)
                {
                    cells[cellIndex + j] = new Point(cellX1 + xOffset, cellY1 + yOffset + (1 * (j * yDir)));
                }

                cellIndex += yInts + 1;
                yOffset += yInts * yDir;
            }
            return cells;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reverse()
        {
            float tempX1 = X1;
            float tempY1 = Y1;

            X1 = X2;
            Y1 = Y2;
            X2 = tempX1;
            Y2 = tempY1;
        }

        public static Line Reverse(Line l1) => new(l1.X2, l1.Y2, l1.X1, l1.Y1);

        public readonly bool Intersects(Line other)
        {

            // this logic wasnt me

            // calculate the direction of the lines
            float uA = ((other.X2 - other.X1) * (Y1 - other.Y1) - (other.Y2 - other.Y1) * (X1 - other.X1)) / ((other.Y2 - other.Y1) * (X2 - X1) - (other.X2 - other.X1) * (Y2 - Y1));
            float uB = ((X2 - X1) * (Y1 - other.Y1) - (Y2 - Y1) * (X1 - other.X1)) / ((other.Y2 - other.Y1) * (X2 - X1) - (other.X2 - other.X1) * (Y2 - Y1));

            // if uA and uB are between 0-1, lines are colliding
            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                return true;
            }
            return false;
        }

        public readonly bool TryGetIntersectionPoint(Line other, out Vector2 intersectionPoint)
        {
            // this logic wasnt me

            // calculate the direction of the lines
            float uA = ((other.X2 - other.X1) * (Y1 - other.Y1) - (other.Y2 - other.Y1) * (X1 - other.X1)) / ((other.Y2 - other.Y1) * (X2 - X1) - (other.X2 - other.X1) * (Y2 - Y1));
            float uB = ((X2 - X1) * (Y1 - other.Y1) - (Y2 - Y1) * (X1 - other.X1)) / ((other.Y2 - other.Y1) * (X2 - X1) - (other.X2 - other.X1) * (Y2 - Y1));

            // if uA and uB are between 0-1, lines are colliding
            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                intersectionPoint = new Vector2(X1 + (uA * (X2 - X1)), Y1 + (uA * (Y2 - Y1)));

                return true;
            }
           
            intersectionPoint = default;
            return false;
        }

        //private readonly Vector2 GetLineDirs(Line other) {

        public static bool TryGetIntersection(Line l1, Line l2, out Vector2 intersectionPoint)
        {
            intersectionPoint = default;
            float d =
            (l2.Y2 - l2.Y1) * (l1.X2 - l1.X1)
            -
            (l2.X2 - l2.X1) * (l1.Y2 - l1.Y1);

            //n_a and n_b are calculated as seperate values for readability
            float n_a =
               (l2.X2 - l2.X1) * (l1.Y1 - l2.Y1)
               -
               (l2.Y2 - l2.Y1) * (l1.X1 - l2.X1);

            float n_b =
               (l1.X2 - l1.X1) * (l1.Y1 - l2.Y1)
               -
               (l1.Y2 - l1.Y1) * (l1.X1 - l2.X1);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.  
            // If n_a and n_b were both equal to zero the lines would be on top of each 
            // other (coincidental).  This check is not done because it is not 
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return false;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            float ua = n_a / d;
            float ub = n_b / d;

            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                intersectionPoint = new Vector2(l1.X1 + (ua * (l1.X2 - l1.X1)), l1.Y1 + (ua * (l1.Y2 - l1.Y1)));
                return true;
            }
            return false;
        }
    }
}
