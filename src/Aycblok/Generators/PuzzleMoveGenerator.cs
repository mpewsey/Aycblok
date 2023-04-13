using MPewsey.Common.Collections;
using MPewsey.Common.Logging;
using MPewsey.Common.Mathematics;
using MPewsey.Common.Pipelines;
using MPewsey.Common.Random;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPewsey.Aycblok.Generators
{
    /// <summary>
    /// A class for procedurally generating the a PuzzleLayout.
    /// </summary>
    public class PuzzleMoveGenerator : IPipelineStep
    {
        private int _targetPushCount = 1;
        /// <summary>
        /// The target number of moves between all push blocks.
        /// </summary>
        public int TargetPushCount { get => _targetPushCount; set => _targetPushCount = Math.Max(value, 1); }

        private int _pushBlockCount = 1;
        /// <summary>
        /// The number of push blocks to generate moves for.
        /// </summary>
        public int PushBlockCount { get => _pushBlockCount; set => _pushBlockCount = Math.Max(value, 1); }

        /// <summary>
        /// If true, the generator will prevent moves where the push block moves in the opposite
        /// direction of its last move.
        /// </summary>
        public bool PreventReversals { get; set; }

        /// <summary>
        /// The maximum number of iterations attempted should the generator initially fail.
        /// </summary>
        public int MaxIterations { get; set; }

        /// <summary>
        /// The random seed.
        /// </summary>
        private RandomSeed RandomSeed { get; set; }

        /// <summary>
        /// The current puzzle layout.
        /// </summary>
        private PuzzleLayout Layout { get; set; }

        /// <summary>
        /// An array of directional offsets.
        /// </summary>
        private Vector2DInt[] Offsets { get; set; }

        /// <summary>
        /// An array of push block indexes.
        /// </summary>
        private int[] PushBlocks { get; set; }

        /// <summary>
        /// Initializes a new generator.
        /// </summary>
        /// <param name="pushBlockCount">The number of push blocks to generate moves for.</param>
        /// <param name="targetPushCount">The target number of moves between all push blocks.</param>
        /// <param name="preventReversals">If true, the generator will prevent moves where the push block moves in the opposite direction of its last move.</param>
        /// <param name="maxIterations">The maximum number of iterations attempted should the generator initially fail.</param>
        public PuzzleMoveGenerator(int pushBlockCount, int targetPushCount, bool preventReversals = true, int maxIterations = 1000)
        {
            PushBlockCount = pushBlockCount;
            TargetPushCount = targetPushCount;
            PreventReversals = preventReversals;
            MaxIterations = maxIterations;
        }

        /// <summary>
        /// Initializes the object for generation.
        /// </summary>
        /// <param name="randomSeed">The random seed.</param>
        private void Initialize(RandomSeed randomSeed)
        {
            RandomSeed = randomSeed;
            PushBlocks = Enumerable.Range(0, PushBlockCount).ToArray();

            Offsets = new Vector2DInt[]
            {
                new Vector2DInt(0, 1),
                new Vector2DInt(0, -1),
                new Vector2DInt(1, 0),
                new Vector2DInt(-1, 0),
            };
        }

        /// <summary>
        /// Applies the generation step. The step takes the following inputs:
        /// 
        /// * PuzzleArea - The input puzzle area. This area should include at least one goal.
        /// * RandomSeed - The random seed.
        /// 
        /// The following artifacts are added to the outputs dictionary:
        /// 
        /// * PuzzleLayout - The generated layout or null if unsuccessful.
        /// </summary>
        /// <param name="results">The generation pipeline results to which artifacts will be added.</param>
        public bool ApplyStep(PipelineResults results)
        {
            var randomSeed = results.GetArgument<RandomSeed>("RandomSeed");
            var cells = results.GetArgument<Array2D<Cell>>("PuzzleArea");
            var layout = GenerateLayout(cells, randomSeed);
            results.SetOutput("PuzzleLayout", layout);
            return layout != null;
        }

        /// <summary>
        /// Generates a new puzzle layout based on the specified input area and returns it.
        /// Returns null if unsuccessful.
        /// </summary>
        /// <param name="cells">The puzzle area cells. These cells should contain at least one goal.</param>
        /// <param name="randomSeed">The random seed.</param>
        public PuzzleLayout GenerateLayout(Array2D<Cell> cells, RandomSeed randomSeed)
        {
            Logger.Log("[Puzzle Move Generator] Generating puzzle moves...");
            Initialize(randomSeed);

            for (int i = 1; i <= MaxIterations; i++)
            {
                Layout = new PuzzleLayout(cells, randomSeed.Seed);
                AddPushBlocks();

                if (AddMoves())
                {
                    Layout.Moves.Reverse();
                    Logger.Log($"[Puzzle Move Generator] Puzzle move generation complete in {i} attempts.");
                    return Layout;
                }

                Logger.Log("[Puzzle Move Generator] Target moves not met. Restarting...");
            }

            Logger.Log("[Puzzle Move Generator] Failed to generate puzzle moves.");
            return null;
        }

        /// <summary>
        /// Returns a list of goal positions that have an empty neighboring cell.
        /// </summary>
        private List<Vector2DInt> FindGoalPositions()
        {
            var positions = Layout.Cells.FindIndexes(x => x.HasFlag(Cell.Goal));
            var result = new List<Vector2DInt>();

            foreach (var position in positions)
            {
                if (AnyNeighborCellIsEmpty(position.X, position.Y))
                    result.Add(position);
            }

            return result;
        }

        /// <summary>
        /// Returns true if any neighbor cell is empty.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        private bool AnyNeighborCellIsEmpty(int row, int column)
        {
            return GetCell(row, column - 1) == Cell.None
                || GetCell(row, column + 1) == Cell.None
                || GetCell(row - 1, column) == Cell.None
                || GetCell(row + 1, column) == Cell.None;
        }

        /// <summary>
        /// Places push blocks on random goal positions.
        /// </summary>
        /// <exception cref="ArgumentException">Raised if no valid goal positions are present.</exception>
        private void AddPushBlocks()
        {
            var positions = FindGoalPositions();

            if (positions.Count == 0)
                throw new ArgumentException("No valid goal positions found.");

            for (int i = 0; i < PushBlockCount; i++)
            {
                var position = positions[RandomSeed.Next(0, positions.Count)];
                Layout.Cells[position] |= Cell.PushBlock;
                Layout.PushBlockPositions.Add(position);
            }
        }

        /// <summary>
        /// Adds moves until the target push count is met. Returns true if successful.
        /// </summary>
        private bool AddMoves()
        {
            foreach (var pushBlock in GetRandomPushBlocks())
            {
                if (!AddMove(pushBlock))
                    return false;
            }

            for (int i = PushBlockCount; i < TargetPushCount; i++)
            {
                if (!AddPushBlockMove())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a move for a random push block. Returns true if successful.
        /// </summary>
        private bool AddPushBlockMove()
        {
            foreach (var pushBlock in GetRandomPushBlocks())
            {
                if (AddMove(pushBlock))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if a cell can be occupied by the pusher.
        /// </summary>
        /// <param name="cell">The cell.</param>
        private static bool PusherCanOccupy(Cell cell)
        {
            var layers = Cell.PushBlock | Cell.BreakBlock | Cell.StopBlock | Cell.OutOfBounds | Cell.PusherVoid;
            return (cell & layers) == Cell.None;
        }

        /// <summary>
        /// Returns true if a cell can be occupied by a push block.
        /// </summary>
        /// <param name="cell">The cell.</param>
        private static bool PushBlockCanOccupy(Cell cell)
        {
            var layers = Cell.PushBlock | Cell.BreakBlock | Cell.StopBlock | Cell.OutOfBounds | Cell.BlockVoid;
            return (cell & layers) == Cell.None;
        }

        /// <summary>
        /// Returns true if the cell can be a stop cell for a push block.
        /// </summary>
        /// <param name="cell">The cell.</param>
        private static bool CanBeStopCell(Cell cell)
        {
            var layers = Cell.PushBlock | Cell.StopBlock;
            return cell == Cell.None || (cell & layers) != Cell.None;
        }

        /// <summary>
        /// Returns true if the pusher can occupy the cell to the back (toward the from position)
        /// of the position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="offset">The offset direction from the to position.</param>
        private bool PusherCanOccupyBackCell(Vector2DInt position, Vector2DInt offset)
        {
            var backPosition = position + offset;
            var backCell = GetCell(backPosition);
            return PusherCanOccupy(backCell);
        }

        /// <summary>
        /// Returns true if one of the side cells can contain and push block, and the other can contain
        /// a stop cell.
        /// </summary>
        /// <param name="position">The cell position.</param>
        /// <param name="offset">The offset direction from the to position.</param>
        private bool SideCellsPermitPushBlock(Vector2DInt position, Vector2DInt offset)
        {
            var sidePosition1 = position + new Vector2DInt(offset.Y, offset.X);
            var sidePosition2 = position + new Vector2DInt(-offset.Y, -offset.X);
            var sideCell1 = GetCell(sidePosition1);
            var sideCell2 = GetCell(sidePosition2);

            return (PushBlockCanOccupy(sideCell1) && CanBeStopCell(sideCell2))
                || (PushBlockCanOccupy(sideCell2) && CanBeStopCell(sideCell1));
        }

        /// <summary>
        /// Returns true if an unobstructed goal is in line with the cell position.
        /// </summary>
        /// <param name="position">The cell position.</param>
        private bool GoalInSight(Vector2DInt position)
        {
            var layers = Cell.PushBlock | Cell.BreakBlock | Cell.StopBlock | Cell.OutOfBounds | Cell.BlockVoid | Cell.Goal;

            foreach (var offset in Offsets)
            {
                var hit = PuzzleBoard.Raycast(Layout.Cells, position, offset, layers);

                if (hit.HitCell.HasFlag(Cell.Goal))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the offset direction is opposite the last move for the push block.
        /// </summary>
        /// <param name="pushBlock">The push block index.</param>
        /// <param name="offset">The offset direction from the to position.</param>
        private bool IsOppositeLastMove(int pushBlock, Vector2DInt offset)
        {
            foreach (var move in Layout.Moves.Reverse<PuzzleMove>())
            {
                if (move.PushBlock == pushBlock)
                    return offset == Vector2DInt.Sign(move.ToPosition - move.FromPosition);
            }

            return false;
        }

        /// <summary>
        /// Shuffles the offsets array an returns it.
        /// </summary>
        private Vector2DInt[] GetRandomOffsets()
        {
            RandomSeed.Shuffle(Offsets);
            return Offsets;
        }

        /// <summary>
        /// Shuffles the push blocks array and returns it.
        /// </summary>
        private int[] GetRandomPushBlocks()
        {
            RandomSeed.Shuffle(PushBlocks);
            return PushBlocks;
        }

        /// <summary>
        /// Accumulates a list of possible push block from positions, shuffles it, and returns it.
        /// </summary>
        /// <param name="pushBlock">The push block index.</param>
        /// <param name="offset">The offset direction from the to position.</param>
        private List<Vector2DInt> GetFromPositions(int pushBlock, Vector2DInt offset)
        {
            var result = new List<Vector2DInt>();
            var toPosition = Layout.PushBlockPositions[pushBlock];
            var cell = GetCell(toPosition);
            var forwardPosition = toPosition - offset;
            var forwardCell = GetCell(forwardPosition);

            // Ensure that a stop cell can be added if the push block is not located on a goal.
            if (!cell.HasFlag(Cell.Goal) && !CanBeStopCell(forwardCell))
                return result;

            // Ensure that the move is not opposite the last if prevent reversals is enabled.
            if (PreventReversals && IsOppositeLastMove(pushBlock, offset))
                return result;

            var layers = Cell.PushBlock | Cell.BreakBlock | Cell.StopBlock | Cell.OutOfBounds | Cell.BlockVoid | Cell.Goal;
            var hit = PuzzleBoard.Raycast(Layout.Cells, toPosition, offset, layers);

            for (var fromPosition = toPosition + offset; fromPosition != hit.Position; fromPosition += offset)
            {
                // Ensure that the block can be pushed.
                if (!PusherCanOccupyBackCell(fromPosition, offset))
                    continue;
                // Ensure the push block can be moved on one of its sides, and that it can be blocked on the other.
                if (!SideCellsPermitPushBlock(fromPosition, offset))
                    continue;
                // Prevent location from being aligned with a goal when it is not located on a goal.
                if (!cell.HasFlag(Cell.Goal) && GoalInSight(fromPosition))
                    continue;

                result.Add(fromPosition);
            }

            RandomSeed.Shuffle(result);
            return result;
        }

        /// <summary>
        /// Returns the cell at the specified position or Out of Bounds.
        /// </summary>
        /// <param name="position">The cell position.</param>
        private Cell GetCell(Vector2DInt position)
        {
            return GetCell(position.X, position.Y);
        }

        /// <summary>
        /// Returns the cell at the specified index or Out of Bounds.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        private Cell GetCell(int row, int column)
        {
            return Layout.Cells.GetOrDefault(row, column, Cell.OutOfBounds);
        }

        /// <summary>
        /// Returns the stop cell type for the push block and offset direction.
        /// </summary>
        /// <param name="pushBlock">The push block.</param>
        /// <param name="offset">The offset direction from the to position.</param>
        private Cell GetStopCell(int pushBlock, Vector2DInt offset)
        {
            var position = Layout.PushBlockPositions[pushBlock];

            if (GetCell(position).HasFlag(Cell.Goal))
                return Cell.Goal;

            var blockPosition = position - offset;
            var blockCell = GetCell(blockPosition);

            if (blockCell.HasFlag(Cell.PushBlock))
                return Cell.PushBlock;
            if (blockCell.HasFlag(Cell.StopBlock))
                return Cell.StopBlock;
            if (IsOppositeLastMove(pushBlock, offset) || Layout.Intersects(blockPosition))
                return Cell.BreakBlock;
            return Cell.StopBlock;
        }

        /// <summary>
        /// Attempts to add a move for the push block. Returns true if successful.
        /// </summary>
        /// <param name="pushBlock">The push block index.</param>
        private bool AddMove(int pushBlock)
        {
            foreach (var offset in GetRandomOffsets())
            {
                foreach (var fromPosition in GetFromPositions(pushBlock, offset))
                {
                    var stopCell = GetStopCell(pushBlock, offset);
                    var toPosition = Layout.PushBlockPositions[pushBlock];
                    var move = new PuzzleMove(pushBlock, stopCell, fromPosition, toPosition);
                    move.InverseApply(Layout.Cells);
                    Layout.PushBlockPositions[pushBlock] = fromPosition;
                    Layout.Moves.Add(move);
                    return true;
                }
            }

            return false;
        }
    }
}
