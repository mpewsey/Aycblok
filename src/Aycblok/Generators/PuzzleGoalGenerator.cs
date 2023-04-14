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
    /// A class for adding a random goal to a puzzle area.
    /// </summary>
    public class PuzzleGoalGenerator : IPipelineStep
    {
        private Vector2DInt _goalSize = Vector2DInt.One;
        /// <summary>
        /// The goal size in rows and columns.
        /// </summary>
        public Vector2DInt GoalSize { get => _goalSize; set => _goalSize = Vector2DInt.Max(value, Vector2DInt.One); }

        /// <summary>
        /// The puzzle area.
        /// </summary>
        private Array2D<PuzzleTile> PuzzleArea { get; set; }

        /// <summary>
        /// The random seed.
        /// </summary>
        private RandomSeed RandomSeed { get; set; }

        /// <summary>
        /// Initializes a new generator.
        /// </summary>
        /// <param name="goalSize">The goal size in rows and columns.</param>
        public PuzzleGoalGenerator(Vector2DInt goalSize)
        {
            GoalSize = goalSize;
        }

        /// <summary>
        /// Initializes the generator.
        /// </summary>
        /// <param name="puzzleArea">The base puzzle area.</param>
        /// <param name="randomSeed">The random seed.</param>
        private void Initialize(Array2D<PuzzleTile> puzzleArea, RandomSeed randomSeed)
        {
            PuzzleArea = new Array2D<PuzzleTile>(puzzleArea);
            RandomSeed = randomSeed;
        }

        /// <summary>
        /// Applies the generation step. The step takes the following inputs:
        /// 
        /// * PuzzleArea - The puzzle area.
        /// * RandomSeed - The random seed.
        /// 
        /// The following results are added to the outputs dictionary:
        /// 
        /// * PuzzleArea - A copy of the puzzle area with the goal added.
        /// </summary>
        /// <param name="results">The generation pipeline results to which artifacts will be added.</param>
        public bool ApplyStep(PipelineResults results)
        {
            var randomSeed = results.GetArgument<RandomSeed>("RandomSeed");
            var puzzleArea = results.GetArgument<Array2D<PuzzleTile>>("PuzzleArea");
            results.SetOutput("PuzzleArea", GenerateGoal(puzzleArea, randomSeed));
            return true;
        }

        /// <summary>
        /// Returns a copy of the puzzle area with a random goal inserted.
        /// </summary>
        /// <param name="puzzleArea">The base puzzle area.</param>
        /// <param name="randomSeed">The random seed.</param>
        public Array2D<PuzzleTile> GenerateGoal(Array2D<PuzzleTile> puzzleArea, RandomSeed randomSeed)
        {
            Logger.Log("[Puzzle Goal Generator] Generating puzzle goals...");
            Initialize(puzzleArea, randomSeed);
            AddGoal();
            Logger.Log("[Puzzle Goal Generator] Goal generation complete.");
            return PuzzleArea;
        }

        /// <summary>
        /// Adds a goal to the puzzle area at a random location.
        /// </summary>
        /// <exception cref="ArgumentException">Raised if no possible goal locations are available.</exception>
        private void AddGoal()
        {
            var positions = FindPossibleGoalPositions();

            if (positions.Count == 0)
                throw new ArgumentException("No possible goal locations found.");

            var index = RandomSeed.Next(0, positions.Count);
            InsertGoal(positions[index]);
        }

        /// <summary>
        /// Inserts a goal into the area at the specified position.
        /// </summary>
        /// <param name="position">The position of the top left corner of the goal.</param>
        private void InsertGoal(Vector2DInt position)
        {
            for (int i = 0; i < GoalSize.X; i++)
            {
                for (int j = 0; j < GoalSize.Y; j++)
                {
                    PuzzleArea[position.X + i, position.Y + j] = PuzzleTile.Goal;
                }
            }
        }

        /// <summary>
        /// Returns a list of all possible goal positions in the area.
        /// </summary>
        private List<Vector2DInt> FindPossibleGoalPositions()
        {
            var result = new List<Vector2DInt>();

            for (int i = 0; i < PuzzleArea.Rows; i++)
            {
                for (int j = 0; j < PuzzleArea.Columns; j++)
                {
                    if (CanAddGoal(i, j))
                        result.Add(new Vector2DInt(i, j));
                }
            }

            return result;
        }

        /// <summary>
        /// Returns true if a goal can be added at the specified index.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        private bool CanAddGoal(int row, int column)
        {
            for (int i = 0; i < GoalSize.X; i++)
            {
                for (int j = 0; j < GoalSize.Y; j++)
                {
                    var tile = PuzzleArea.GetOrDefault(row + i, column + j, PuzzleTile.OutOfBounds);

                    if (tile != PuzzleTile.None)
                        return false;
                }
            }

            return true;
        }
    }
}
