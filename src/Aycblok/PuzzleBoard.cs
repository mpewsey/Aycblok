using MPewsey.Common.Collections;
using MPewsey.Common.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPewsey.Aycblok
{
    /// <summary>
    /// Contains general methods pertaining to areas of tiles, or puzzle boards.
    /// </summary>
    public static class PuzzleBoard
    {
        /// <summary>
        /// Performs a raycast in the specified offset direction until the first blocking layer is met.
        /// </summary>
        /// <param name="tiles">The array of tiles.</param>
        /// <param name="position">The starting position. This position is not included in the raycast.</param>
        /// <param name="offset">The offset direction.</param>
        /// <param name="blockingLayers">The tile layers that block the raycast.</param>
        /// <exception cref="ArgumentException">Raised if a zero vector is supplied as the offset.</exception>
        public static RaycastHit Raycast(Array2D<PuzzleTile> tiles, Vector2DInt position, Vector2DInt offset, PuzzleTile blockingLayers)
        {
            if (offset == Vector2DInt.Zero)
                throw new ArgumentException("Offset must be non-zero vector.");

            var tile = PuzzleTile.None;
            blockingLayers |= PuzzleTile.OutOfBounds;

            while ((tile & blockingLayers) == PuzzleTile.None)
            {
                position += offset;
                tile = tiles.GetOrDefault(position.X, position.Y, PuzzleTile.OutOfBounds);
            }

            return new RaycastHit(tile, position);
        }

        /// <summary>
        /// Returns the string representation for a puzzle board.
        /// </summary>
        /// <param name="tiles">The puzzle board tiles.</param>
        public static string GetString(Array2D<PuzzleTile> tiles)
        {
            var size = 2 * tiles.Array.Length + tiles.Rows;
            var builder = new StringBuilder(size);
            AddString(tiles, builder);
            return builder.ToString();
        }

        /// <summary>
        /// Returns the puzzle board strings in a tiled layout.
        /// </summary>
        /// <param name="tiles">A list of move puzzle boards.</param>
        /// <param name="columns">The number of columns in the layout.</param>
        public static string GetTiledString(IList<Array2D<PuzzleTile>> tiles, int columns)
        {
            if (tiles.Count == 0)
                return string.Empty;

            const int spaceCount = 3;
            columns = Math.Max(columns, 1);
            var builder = new StringBuilder();
            var width = 2 * tiles[0].Columns + spaceCount;
            var rows = (int)Math.Ceiling(tiles.Count / (double)columns);

            for (int row = 0; row < rows; row++)
            {
                // Add headers
                for (int column = 0; column < columns; column++)
                {
                    var index = row * columns + column;

                    if (index >= tiles.Count)
                        break;

                    var header = index == 0 ? "Start board:" : $"Move {index}:";
                    builder.Append(header);

                    if (column < columns - 1)
                    {
                        var length = Math.Max(width - header.Length, 0);
                        builder.Append(' ', length);
                    }
                }

                builder.Append('\n');

                // Add tile strings
                for (int i = 0; i < tiles[0].Rows; i++)
                {
                    for (int column = 0; column < columns; column++)
                    {
                        var index = row * columns + column;

                        if (index >= tiles.Count)
                            break;

                        // Add row tile strings
                        for (int j = 0; j < tiles[0].Columns; j++)
                        {
                            builder.Append(GetTileCharacter(tiles[index][i, j])).Append(' ');
                        }

                        // Add separator
                        if (column < columns - 1)
                            builder.Append(' ', spaceCount);
                    }

                    builder.Append('\n');
                }

                if (row < rows - 1)
                    builder.Append("\n");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Adds the tiles string to the string builder.
        /// </summary>
        /// <param name="tiles">The puzzle board tiles.</param>
        /// <param name="builder">The string builder.</param>
        public static void AddString(Array2D<PuzzleTile> tiles, StringBuilder builder)
        {
            for (int i = 0; i < tiles.Rows; i++)
            {
                for (int j = 0; j < tiles.Columns; j++)
                {
                    builder.Append(GetTileCharacter(tiles[i, j])).Append(' ');
                }

                builder.Append('\n');
            }
        }

        /// <summary>
        /// Returns the character corresponding to the tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <exception cref="ArgumentException">Raised if the tile value is unhandled.</exception>
        private static char GetTileCharacter(PuzzleTile tile)
        {
            var layers = PuzzleTile.StopBlock | PuzzleTile.BreakBlock | PuzzleTile.PushBlock | PuzzleTile.Goal | PuzzleTile.Void;

            switch (tile & layers)
            {
                case PuzzleTile.None:
                    return '.';
                case PuzzleTile.StopBlock:
                    return '#';
                case PuzzleTile.BreakBlock:
                    return '%';
                case PuzzleTile.PushBlock:
                    return 'o';
                case PuzzleTile.Goal:
                    return '$';
                case PuzzleTile.Goal | PuzzleTile.PushBlock:
                    return '*';
                case PuzzleTile.Void:
                    return '!';
                case PuzzleTile.BlockVoid:
                    return '+';
                case PuzzleTile.PusherVoid:
                    return '@';
                default:
                    throw new ArgumentException($"Unhandled tile type: {tile}.");
            }
        }
    }
}
