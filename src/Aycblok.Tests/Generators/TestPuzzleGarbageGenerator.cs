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
            var area = new Array2D<Cell>(21, 21);
            area[10, 10] = Cell.Goal;
            Console.WriteLine("Starting Area:");
            Console.WriteLine(PuzzleBoard.GetString(area));

            var layout = new PuzzleLayout(area, randomSeed.Seed);
            var generator = new PuzzleGarbageGenerator(0.5f, 0.5f);
            generator.GenerateGarbage(layout, randomSeed);
            Console.WriteLine(layout.GetMoveReport());
            Assert.IsTrue(layout.Cells.Array.Contains(Cell.BreakBlock | Cell.Garbage));
            Assert.IsTrue(layout.Cells.Array.Contains(Cell.StopBlock | Cell.Garbage));

            Logger.RemoveAllListeners();
        }
    }
}