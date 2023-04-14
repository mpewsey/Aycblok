using MPewsey.Common.Mathematics;

namespace MPewsey.Aycblok
{
    /// <summary>
    /// A container for storing the results of a raycast.
    /// </summary>
    public struct RaycastHit
    {
        /// <summary>
        /// The value of the blocking tile.
        /// </summary>
        public PuzzleTile HitTile { get; }

        /// <summary>
        /// The position of the blocking tile.
        /// </summary>
        public Vector2DInt Position { get; }

        /// <summary>
        /// Initializes a new raycast hit.
        /// </summary>
        /// <param name="hitTile">The value of the blocking tile.</param>
        /// <param name="position">The position of the blocking tile.</param>

        public RaycastHit(PuzzleTile hitTile, Vector2DInt position)
        {
            HitTile = hitTile;
            Position = position;
        }

        public override string ToString()
        {
            return $"RaycastHit(HitTile = {HitTile}, Position = {Position})";
        }
    }
}
