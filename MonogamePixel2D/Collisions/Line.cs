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

        public Line(Vector2 start, Vector2 end) : this(start.X, start.Y, end.X, end.Y) { }
        public Line(Vector4 lineVec) : this(lineVec.X, lineVec.Y, lineVec.Z, lineVec.W) { }

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
