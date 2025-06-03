using Microsoft.Xna.Framework;

namespace MonoGamePixel2D.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="Matrix"/> class, specifically for 2D transformations.
    /// </summary>
    public static class Matrix2DExtensions
    {
        /// <summary>
        /// Sets the 2D translation of the matrix (xy).
        /// </summary>
        /// <param name="matrix">The matrix to set the 2D translation of.</param>
        /// <param name="translation">The <see cref="Vector2"/> to set the translation to.</param>
        public static Matrix WithTranslation2D(this Matrix matrix, Vector2 translation)
        {
            matrix.M41 = translation.X;
            matrix.M42 = translation.Y;
            return matrix;
        }

        /// <summary>
        /// Sets the 2D translation of the matrix (xy).
        /// </summary>
        /// <param name="matrix">The matrix to set the 2D translation of.</param>
        /// <param name="x">X of the translation.</param>
        /// <param name="y">Y pf the translation.</param>
        public static Matrix WithTranslation2D(this Matrix matrix, float x, float y)
        {
            matrix.M41 = x;
            matrix.M42 = y;
            return matrix;
        }

        /// <summary>
        /// Gets the 2D translation of the matrix (xy).
        /// </summary>
        /// <param name="matrix">The matrix to get the 2D translation from.</param>
        /// <returns>2D Translation as a <see cref="Vector2"/>.</returns>
        public static Vector2 GetTranslation2D(this Matrix matrix) =>
            new(matrix.M41, matrix.M42);

        /// <summary>
        /// Gets the 2D translation of the matrix (xy).
        /// </summary>
        /// <param name="matrix">The matrix to get the 2D translation from.</param>
        /// <param name="x">The x value of the matrix's 2D translation.</param>
        /// <param name="y">The y value of the matrix's 2D translation.</param>
        public static void GetTranslation2D(this Matrix matrix, out float x, out float y)
        {
            x = matrix.M41;
            y = matrix.M42;
        }

        public static Matrix WithScale2D(this Matrix matrix, Vector2 scale)
        {
            matrix.M11 = scale.X;
            matrix.M22 = scale.Y;
            return matrix;
        }

        public static Vector2 GetScale2D(this Matrix matrix) => new(matrix.M11, matrix.M22);
    }
}
