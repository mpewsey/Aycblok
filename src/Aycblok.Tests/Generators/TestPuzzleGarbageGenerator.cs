using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPewsey.Common.Collections;
using MPewsey.Common.Logging;
using MPewsey.Common.Random;
using System;
using System.Linq;

namespace MPewsey.Aycblok.Generators.Tests
{
    [TestClass]
    public class TestPuzzleGarbageGenerator
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
            area[10, 10] = PuzzleTile.Goal;
            Console.WriteLine("Starting Area:");
            Console.WriteLine(PuzzleBoard.TilesToString(area));

            var layout = new PuzzleLayout(area, randomSeed.Seed);
            var generator = new PuzzleGarbageGenerator(0.5f, 0.5f);
            generator.GenerateGarbage(layout, randomSeed);
            Console.WriteLine(layout.MoveReport());
            Assert.IsTrue(layout.Tiles.Array.Contains(PuzzleTile.BreakBlock | PuzzleTile.Garbage));
            Assert.IsTrue(layout.Tiles.Array.Contains(PuzzleTile.StopBlock | PuzzleTile.Garbage));

            Logger.RemoveAllListeners();
        }
    }
}