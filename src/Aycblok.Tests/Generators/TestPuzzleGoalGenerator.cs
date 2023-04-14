using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPewsey.Common.Collections;
using MPewsey.Common.Logging;
using MPewsey.Common.Mathematics;
using MPewsey.Common.Random;
using System;
using System.Linq;

namespace MPewsey.Aycblok.Generators.Tests
{
    [TestClass]
    public class TestPuzzleGoalGenerator
    {
        [DataTestMethod]
        [DataRow(12345)]
        [DataRow(56789)]
        [DataRow(31415)]
        public void TestGenerate(int seed)
        {
            Logger.RemoveAllListeners();
            Logger.AddListener(Console.WriteLine);

            var randomSeed = new RandomSeed(seed);
            var area = new Array2D<PuzzleTile>(21, 21);
            Console.WriteLine("Starting Area:");
            Console.WriteLine(PuzzleBoard.GetString(area));

            var generator = new PuzzleGoalGenerator(new Vector2DInt(2, 2));
            var result = generator.GenerateGoal(area, randomSeed);
            Console.WriteLine("Result:");
            Console.WriteLine(PuzzleBoard.GetString(result));

            Assert.AreEqual(4, result.Array.Count(x => x == PuzzleTile.Goal));
            Logger.RemoveAllListeners();
        }
    }
}