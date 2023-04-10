using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPewsey.Common.Collections;
using MPewsey.Common.Logging;
using MPewsey.Common.Random;
using MPewsey.Common.Serialization;
using System;

namespace MPewsey.Aycblok.Generators.Tests
{
    [TestClass]
    public class TestPuzzleMoveGenerator
    {
        [DataTestMethod]
        [DataRow(12345)]
        [DataRow(56789)]
        [DataRow(31415)]
        [DataRow(61415)]
        [DataRow(71415)]
        [DataRow(81415)]
        public void TestGenerateSmallLayout(int seed)
        {
            Logger.RemoveAllListeners();
            Logger.AddListener(Console.WriteLine);

            var area = new Array2D<Cell>(9, 9);
            area[4, 4] = Cell.Goal;
            Console.WriteLine("Starting Area:");
            Console.WriteLine(PuzzleBoard.GetString(area));

            var randomSeed = new RandomSeed(seed);
            var generator = new PuzzleMoveGenerator(1, 8);
            var layout = generator.GenerateLayout(area, randomSeed);
            Assert.IsNotNull(layout);

            Console.WriteLine(layout.GetTiledMoveReport(3));
            Logger.RemoveAllListeners();

            XmlSerialization.SaveXml($"SmallLayout{seed}.xml", layout);
            JsonSerialization.SaveJson($"SmallLayout{seed}.json", layout);
        }

        [DataTestMethod]
        [DataRow(12345)]
        [DataRow(56789)]
        [DataRow(31415)]
        [DataRow(61415)]
        [DataRow(71415)]
        [DataRow(81415)]
        public void TestGenerateSimpleLayout(int seed)
        {
            Logger.RemoveAllListeners();
            Logger.AddListener(Console.WriteLine);

            var area = new Array2D<Cell>(21, 21);
            area[10, 10] = Cell.Goal;
            Console.WriteLine("Starting Area:");
            Console.WriteLine(PuzzleBoard.GetString(area));

            var randomSeed = new RandomSeed(seed);
            var generator = new PuzzleMoveGenerator(1, 10);
            var layout = generator.GenerateLayout(area, randomSeed);
            Assert.IsNotNull(layout);

            Console.WriteLine(layout.GetMoveReport());
            Logger.RemoveAllListeners();

            XmlSerialization.SaveXml($"SimpleLayout{seed}.xml", layout);
            JsonSerialization.SaveJson($"SimpleLayout{seed}.json", layout);
        }

        [DataTestMethod]
        [DataRow(12345)]
        [DataRow(56789)]
        [DataRow(31415)]
        [DataRow(61415)]
        [DataRow(71415)]
        [DataRow(81415)]
        public void TestGenerateSimpleLayoutWithReversals(int seed)
        {
            Logger.RemoveAllListeners();
            Logger.AddListener(Console.WriteLine);

            var area = new Array2D<Cell>(21, 21);
            area[10, 10] = Cell.Goal;
            Console.WriteLine("Starting Area:");
            Console.WriteLine(PuzzleBoard.GetString(area));

            var randomSeed = new RandomSeed(seed);
            var generator = new PuzzleMoveGenerator(1, 10, false);
            var layout = generator.GenerateLayout(area, randomSeed);
            Assert.IsNotNull(layout);

            Console.WriteLine(layout.GetMoveReport());
            Logger.RemoveAllListeners();
        }

        [DataTestMethod]
        [DataRow(12345)]
        [DataRow(56789)]
        [DataRow(31415)]
        public void TestGenerateBigGoalLayout(int seed)
        {
            Logger.RemoveAllListeners();
            Logger.AddListener(Console.WriteLine);

            var area = new Array2D<Cell>(21, 21);
            area[10, 10] = Cell.Goal;
            area[10, 11] = Cell.Goal;
            area[11, 10] = Cell.Goal;
            area[11, 11] = Cell.Goal;

            Console.WriteLine("Starting Area:");
            Console.WriteLine(PuzzleBoard.GetString(area));

            var randomSeed = new RandomSeed(seed);
            var generator = new PuzzleMoveGenerator(3, 50, true);
            var layout = generator.GenerateLayout(area, randomSeed);
            Assert.IsNotNull(layout);

            Console.WriteLine(layout.GetMoveReport());
            Logger.RemoveAllListeners();
        }
    }
}