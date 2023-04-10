using MPewsey.Common.Collections;
using MPewsey.Common.Logging;
using MPewsey.Common.Mathematics;
using MPewsey.Common.Pipelines;
using MPewsey.Common.Random;
using System;
using System.Collections.Generic;

namespace MPewsey.Aycblok.Generators
{
    /// <summary>
    /// A class for adding random garbage blocks to a PuzzleLayout.
    /// </summary>
    public class PuzzleGarbageGenerator : IPipelineStep
    {
        /// <summary>
        /// The target density of garbage blocks in the open area.
        /// </summary>
        public float TargetDensity { get; set; }

        /// <summary>
        /// The chance that a garbage block will be a break block.
        /// </summary>
        public float BreakBlockChance { get; set; }

        /// <summary>
        /// The random seed.
        /// </summary>
        private RandomSeed RandomSeed { get; set; }

        /// <summary>
        /// The puzzle layout.
        /// </summary>
        private PuzzleLayout Layout { get; set; }

        /// <summary>
        /// Initializes a new generator.
        /// </summary>
        /// <param name="targetDensity">The target density of garbage blocks in the open area.</param>
        /// <param name="breakBlockChance">The chance that a garbage block will be a break block.</param>
        public PuzzleGarbageGenerator(float targetDensity, float breakBlockChance)
        {
            TargetDensity = targetDensity;
            BreakBlockChance = breakBlockChance;
        }

        /// <summary>
        /// Initializes the generator.
        /// </summary>
        /// <param name="layout">The puzzle layout.</param>
        /// <param name="randomSeed">The random seed.</param>
        private void Initialize(PuzzleLayout layout, RandomSeed randomSeed)
        {
            Layout = layout;
            RandomSeed = randomSeed;
        }

        /// <summary>
        /// Applies the generation step. The step takes the following inputs:
        /// 
        /// * PuzzleLayout - The puzzle layout to which garbage blocks will be added.
        /// * RandomSeed - The random seed.
        /// </summary>
        /// <param name="results">The generation pipeline results to which artifacts will be added.</param>
        public bool ApplyStep(PipelineResults results)
        {
            var randomSeed = results.GetArgument<RandomSeed>("RandomSeed");
            var layout = results.GetArgument<PuzzleLayout>("PuzzleLayout");
            GenerateGarbage(layout, randomSeed);
            return true;
        }

        /// <summary>
        /// Adds random garbage blocks to the layout. The added garbage includes the garbage cell layer.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="randomSeed">The random seed.</param>
        public void GenerateGarbage(PuzzleLayout layout, RandomSeed randomSeed)
        {
            Logger.Log("Generating garbage blocks...");
            Initialize(layout, randomSeed);
            AddGarbage();
            Logger.Log("Garbage block generation complete.");
        }

        /// <summary>
        /// Adds garbage to the layout at random positions until the target density is met or
        /// all positions are consumed.
        /// </summary>
        private void AddGarbage()
        {
            var positions = FindOpenPositions();
            var targetBlocks = Math.Min(TargetGarbageBlocks(positions.Count), positions.Count);
            RandomSeed.Shuffle(positions);

            for (int i = 0; i < targetBlocks; i++)
            {
                var position = positions[i];
                Layout.Cells[position] |= GetRandomBlock();
            }
        }

        /// <summary>
        /// Returns an array of cells with move paths marked.
        /// </summary>
        private Array2D<Cell> GetMarkedCells()
        {
            var cells = new Array2D<Cell>(Layout.Cells);

            foreach (var move in Layout.Moves)
            {
                cells[move.PushCellPosition()] |= Cell.Marked;
                cells[move.StopCellPosition()] |= Cell.Marked;

                var min = Vector2DInt.Min(move.FromPosition, move.ToPosition);
                var max = Vector2DInt.Max(move.FromPosition, move.ToPosition);

                for (int i = min.X; i <= max.X; i++)
                {
                    for (int j = min.Y; j <= max.Y; j++)
                    {
                        cells[i, j] |= Cell.Marked;
                    }
                }
            }

            return cells;
        }

        /// <summary>
        /// Returns a list of open positions where blocks can be placed without blocking the puzzle.
        /// </summary>
        private List<Vector2DInt> FindOpenPositions()
        {
            return GetMarkedCells().FindIndexes(x => x == Cell.None);
        }

        /// <summary>
        /// Returns a random stop block type based on the break block chance.
        /// </summary>
        private Cell GetRandomBlock()
        {
            if (RandomSeed.ChanceSatisfied(BreakBlockChance))
                return Cell.BreakBlock | Cell.Garbage;
            return Cell.StopBlock | Cell.Garbage;
        }

        /// <summary>
        /// Returns the number of garbage blocks necessary to meet or exceed the target density.
        /// </summary>
        /// <param name="openArea">The number of open cells in the layout.</param>
        private int TargetGarbageBlocks(int openArea)
        {
            return (int)Math.Max(Math.Ceiling(TargetDensity * (double)openArea), 0);
        }
    }
}
