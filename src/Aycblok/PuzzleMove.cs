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
        public Cell StopCell { get; private set; }

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
        /// <param name="stopCell">The type of block serving to stop the push block.</param>
        /// <param name="fromPosition">The starting position of the push block.</param>
        /// <param name="toPosition">The final position of the push block.</param>

        public PuzzleMove(int pushBlock, Cell stopCell, Vector2DInt fromPosition, Vector2DInt toPosition)
        {
            PushBlock = pushBlock;
            StopCell = stopCell;
            FromPosition = fromPosition;
            ToPosition = toPosition;
        }

        public override string ToString()
        {
            return $"PuzzleMove(PushBlock = {PushBlock}, StopCell = {StopCell}, FromPosition = {FromPosition}, ToPosition = {ToPosition})";
        }

        /// <summary>
        /// Modifies the specified puzzle board to move the push block from the from position to the to position.
        /// </summary>
        /// <param name="cells">The puzzle board.</param>
        public void Apply(Array2D<Cell> cells)
        {
            cells[StopCellPosition()] &= ~Cell.BreakBlock;
            cells[ToPosition] |= Cell.PushBlock;
            cells[FromPosition] &= ~Cell.PushBlock;
        }

        /// <summary>
        /// Modifies the specified puzzle board to move the push block from the to position to the from position.
        /// </summary>
        /// <param name="cells">The puzzle board.</param>
        public void InverseApply(Array2D<Cell> cells)
        {
            cells[StopCellPosition()] |= StopCell;
            cells[FromPosition] |= Cell.PushBlock;
            cells[ToPosition] &= ~Cell.PushBlock;
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
        /// The position of the stop cell.
        /// </summary>
        public Vector2DInt StopCellPosition()
        {
            return ToPosition + StopCellOffset();
        }

        /// <summary>
        /// The position of the push cell.
        /// </summary>
        public Vector2DInt PushCellPosition()
        {
            return FromPosition + PushCellOffset();
        }

        /// <summary>
        /// The offset of the stop cell from the to position.
        /// </summary>
        private Vector2DInt StopCellOffset()
        {
            if (StopCell == Cell.Goal)
                return Vector2DInt.Zero;
            return Vector2DInt.Sign(ToPosition - FromPosition);
        }

        /// <summary>
        /// The offset of the push cell from the from position.
        /// </summary>
        private Vector2DInt PushCellOffset()
        {
            return Vector2DInt.Sign(FromPosition - ToPosition);
        }
    }
}
