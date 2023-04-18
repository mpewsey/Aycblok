using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPewsey.Common.Collections;
using System;

namespace MPewsey.Aycblok.Generators.Tests
{
    [TestClass]
    public class TestPuzzleBoard
    {
        [TestMethod]
        public void TestStringsToTiles()
        {
            var lines = new string[]
            {
                ".#!@+*$%o",
                "o%$*+@!#.",
            };

            var tiles = new PuzzleTile[,]
            {
                { PuzzleTile.None, PuzzleTile.StopBlock, PuzzleTile.Void, PuzzleTile.PusherVoid, PuzzleTile.BlockVoid,
                    PuzzleTile.Goal | PuzzleTile.PushBlock, PuzzleTile.Goal, PuzzleTile.BreakBlock, PuzzleTile.PushBlock },
                { PuzzleTile.PushBlock, PuzzleTile.BreakBlock, PuzzleTile.Goal, PuzzleTile.Goal | PuzzleTile.PushBlock,
                    PuzzleTile.BlockVoid, PuzzleTile.PusherVoid, PuzzleTile.Void, PuzzleTile.StopBlock, PuzzleTile.None },
            };

            var expected = new Array2D<PuzzleTile>(tiles);
            var result = PuzzleBoard.StringsToTiles(lines);
            Console.WriteLine("Expected:");
            Console.WriteLine(PuzzleBoard.TilesToString(expected));
            Console.WriteLine("Result:");
            Console.WriteLine(PuzzleBoard.TilesToString(result));
            CollectionAssert.AreEqual(expected.Array, result.Array);
        }

        [TestMethod]
        public void TestTilesToString()
        {
            var tiles = new PuzzleTile[,]
            {
                { PuzzleTile.None, PuzzleTile.StopBlock, PuzzleTile.Void, PuzzleTile.PusherVoid, PuzzleTile.BlockVoid,
                    PuzzleTile.Goal | PuzzleTile.PushBlock, PuzzleTile.Goal, PuzzleTile.BreakBlock, PuzzleTile.PushBlock },
                { PuzzleTile.PushBlock, PuzzleTile.BreakBlock, PuzzleTile.Goal, PuzzleTile.Goal | PuzzleTile.PushBlock,
                    PuzzleTile.BlockVoid, PuzzleTile.PusherVoid, PuzzleTile.Void, PuzzleTile.StopBlock, PuzzleTile.None },
            };

            var expected = new string[]
            {
                ".#!@+*$%o",
                "o%$*+@!#.",
            };

            var result = PuzzleBoard.TilesToString(tiles).Replace(" ", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
            CollectionAssert.AreEqual(expected, result);
        }
    }
}