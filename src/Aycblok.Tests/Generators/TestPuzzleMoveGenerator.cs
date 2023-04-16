using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPewsey.Common.Collections;
using MPewsey.Common.Logging;
using MPewsey.Common.Mathematics;
using MPewsey.Common.Pipelines;
using MPewsey.Common.Random;
using MPewsey.Common.Serialization;
using System;
using System.Collections.Generic;

namespace MPewsey.Aycblok.Generators.Tests
{
    [TestClass]
    public class TestPuzzleMoveGenerator
    {
        [TestMethod]
        public void TestPipelineGenerateSmallLayout()
        {
            Logger.RemoveAllListeners();
            Logger.AddListener(Console.WriteLine);
            var seed = new RandomSeed(12345);
            var area = new Array2D<PuzzleTile>(9, 9);

            var args = new Dictionary<string, object>
            {
                { "PuzzleArea", area },
                { "RandomSeed", seed },
            };

            var pipeline = new Pipeline(
                new PuzzleGoalGenerator(Vector2DInt.One),
                new PuzzleMoveGenerator(1, 8),
                new PuzzleGarbageGenerator(0.1f, 0.5f)
            );

            var results = pipeline.Run(args);
            var layout = results.GetOutput<PuzzleLayout>("PuzzleLayout");
            Assert.IsNotNull(layout);

            Console.WriteLine(layout.GetTiledMoveReport(3));
            Logger.RemoveAllListeners();
        }

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

            var area = new Array2D<PuzzleTile>(9, 9);
            area[4, 4] = PuzzleTile.Goal;
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

            var area = new Array2D<PuzzleTile>(21, 21);
            area[10, 10] = PuzzleTile.Goal;
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

            var area = new Array2D<PuzzleTile>(21, 21);
            area[10, 10] = PuzzleTile.Goal;
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

            var area = new Array2D<PuzzleTile>(21, 21);
            area[10, 10] = PuzzleTile.Goal;
            area[10, 11] = PuzzleTile.Goal;
            area[11, 10] = PuzzleTile.Goal;
            area[11, 11] = PuzzleTile.Goal;

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