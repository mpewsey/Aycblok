using MPewsey.Common.Mathematics;

namespace MPewsey.Aycblok
{
    /// <summary>
    /// A container for storing the results of a raycast.
    /// </summary>
    public struct RaycastHit
    {
        /// <summary>
        /// The value of the blocking cell.
        /// </summary>
        public Cell HitCell { get; }

        /// <summary>
        /// The position of the blocking cell.
        /// </summary>
        public Vector2DInt Position { get; }

        /// <summary>
        /// Initializes a new raycast hit.
        /// </summary>
        /// <param name="hitCell">The value of the blocking cell.</param>
        /// <param name="position">The position of the blocking cell.</param>

        public RaycastHit(Cell hitCell, Vector2DInt position)
        {
            HitCell = hitCell;
            Position = position;
        }

        public override string ToString()
        {
            return $"RaycastHit(HitCell = {HitCell}, Position = {Position})";
        }
    }
}
