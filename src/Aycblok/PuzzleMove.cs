using MPewsey.Common.Collections;
using MPewsey.Common.Mathematics;
using System.Runtime.Serialization;

namespace MPewsey.Aycblok
{
    /// <summary>
    /// A containing with informating pertaining to the move of a push block.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class PuzzleMove
    {
        /// <summary>
        /// The push block.
        /// </summary>
        [DataMember(Order = 1)]
        public int PushBlock { get; private set; }

        /// <summary>
        /// The type of block serving to stop the push block.
        /// </summary>
        [DataMember(Order = 2)]
        public PuzzleTile StopTile { get; private set; }

        /// <summary>
        /// The starting position of the push block.
        /// </summary>
        [DataMember(Order = 3)]
        public Vector2DInt FromPosition { get; private set; }

        /// <summary>
        /// The final position of the push block.
        /// </summary>
        [DataMember(Order = 4)]
        public Vector2DInt ToPosition { get; private set; }

        /// <summary>
        /// Initializes a new move.
        /// </summary>
        /// <param name="pushBlock">The push block.</param>
        /// <param name="stopTile">The type of block serving to stop the push block.</param>
        /// <param name="fromPosition">The starting position of the push block.</param>
        /// <param name="toPosition">The final position of the push block.</param>

        public PuzzleMove(int pushBlock, PuzzleTile stopTile, Vector2DInt fromPosition, Vector2DInt toPosition)
        {
            PushBlock = pushBlock;
            StopTile = stopTile;
            FromPosition = fromPosition;
            ToPosition = toPosition;
        }

        public override string ToString()
        {
            return $"PuzzleMove(PushBlock = {PushBlock}, StopTile = {StopTile}, FromPosition = {FromPosition}, ToPosition = {ToPosition})";
        }

        /// <summary>
        /// Modifies the specified puzzle board to move the push block from the from position to the to position.
        /// </summary>
        /// <param name="tiles">The puzzle board.</param>
        public void Apply(Array2D<PuzzleTile> tiles)
        {
            tiles[StopTilePosition()] &= ~PuzzleTile.BreakBlock;
            tiles[ToPosition] |= PuzzleTile.PushBlock;
            tiles[FromPosition] &= ~PuzzleTile.PushBlock;
        }

        /// <summary>
        /// Modifies the specified puzzle board to move the push block from the to position to the from position.
        /// </summary>
        /// <param name="tiles">The puzzle board.</param>
        public void InverseApply(Array2D<PuzzleTile> tiles)
        {
            tiles[StopTilePosition()] |= StopTile;
            tiles[FromPosition] |= PuzzleTile.PushBlock;
            tiles[ToPosition] &= ~PuzzleTile.PushBlock;
        }

        /// <summary>
        /// Returns true if the specified position intersects the move's push path.
        /// </summary>
        /// <param name="position">The position.</param>
        public bool Intersects(Vector2DInt position)
        {
            var min = Vector2DInt.Min(FromPosition, ToPosition);
            var max = Vector2DInt.Max(FromPosition, ToPosition);

            return position.X >= min.X && position.X <= max.X
                && position.Y >= min.Y && position.Y <= max.Y;
        }

        /// <summary>
        /// The position of the stop tile.
        /// </summary>
        public Vector2DInt StopTilePosition()
        {
            return ToPosition + StopTileOffset();
        }

        /// <summary>
        /// The position of the push tile.
        /// </summary>
        public Vector2DInt PushTilePosition()
        {
            return FromPosition + PushTileOffset();
        }

        /// <summary>
        /// The offset of the stop tile from the to position.
        /// </summary>
        private Vector2DInt StopTileOffset()
        {
            if (StopTile == PuzzleTile.Goal)
                return Vector2DInt.Zero;
            return Vector2DInt.Sign(ToPosition - FromPosition);
        }

        /// <summary>
        /// The offset of the push tile from the from position.
        /// </summary>
        private Vector2DInt PushTileOffset()
        {
            return Vector2DInt.Sign(FromPosition - ToPosition);
        }
    }
}
