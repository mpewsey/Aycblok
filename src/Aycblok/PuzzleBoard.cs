using MPewsey.Common.Collections;
using MPewsey.Common.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPewsey.Aycblok
{
    /// <summary>
    /// Contains general methods pertaining to areas of cells, or puzzle boards.
    /// </summary>
    public static class PuzzleBoard
    {
        /// <summary>
        /// Performs a raycast in the specified offset direction until the first blocking layer is met.
        /// </summary>
        /// <param name="cells">The array of cells.</param>
        /// <param name="position">The starting position. This position is not included in the raycast.</param>
        /// <param name="offset">The offset direction.</param>
        /// <param name="blockingLayers">The cell layers that block the raycast.</param>
        /// <exception cref="ArgumentException">Raised if a zero vector is supplied as the offset.</exception>
        public static RaycastHit Raycast(Array2D<Cell> cells, Vector2DInt position, Vector2DInt offset, Cell blockingLayers)
        {
            if (offset == Vector2DInt.Zero)
                throw new ArgumentException("Offset must be non-zero vector.");

            var cell = Cell.None;
            blockingLayers |= Cell.OutOfBounds;

            while ((cell & blockingLayers) == Cell.None)
            {
                position += offset;
                cell = cells.GetOrDefault(position.X, position.Y, Cell.OutOfBounds);
            }

            return new RaycastHit(cell, position);
        }

        /// <summary>
        /// Returns the string representation for a puzzle board.
        /// </summary>
        /// <param name="cells">The puzzle board cells.</param>
        public static string GetString(Array2D<Cell> cells)
        {
            var size = 2 * cells.Array.Length + cells.Rows;
            var builder = new StringBuilder(size);
            AddString(cells, builder);
            return builder.ToString();
        }

        /// <summary>
        /// Returns the puzzle board strings in a tiled layout.
        /// </summary>
        /// <param name="cells">A list of move puzzle boards.</param>
        /// <param name="columns">The number of columns in the layout.</param>
        public static string GetTiledString(IList<Array2D<Cell>> cells, int columns)
        {
            if (cells.Count == 0)
                return string.Empty;

            const int spaceCount = 3;
            columns = Math.Max(columns, 1);
            var builder = new StringBuilder();
            var width = 2 * cells[0].Columns + spaceCount;
            var rows = (int)Math.Ceiling(cells.Count / (double)columns);

            for (int m = 0; m < rows; m++)
            {
                // Add headers
                for (int n = 0; n < columns; n++)
                {
                    var k = m * columns + n;

                    if (k >= cells.Count)
                        break;

                    var header = k == 0 ? "Start board:" : $"Move {k}:";
                    builder.Append(header);

                    if (n < columns - 1)
                    {
                        var length = Math.Max(width - header.Length, 0);
                        builder.Append(' ', length);
                    }
                }

                builder.Append('\n');

                // Add cell strings
                for (int i = 0; i < cells[0].Rows; i++)
                {
                    for (int n = 0; n < columns; n++)
                    {
                        var k = m * columns + n;

                        if (k >= cells.Count)
                            break;

                        // Add row cell strings
                        for (int j = 0; j < cells[0].Columns; j++)
                        {
                            builder.Append(GetCellCharacter(cells[k][i, j])).Append(' ');
                        }

                        // Add separator
                        if (n < columns - 1)
                            builder.Append(' ', spaceCount);
                    }

                    builder.Append('\n');
                }

                if (m < rows - 1)
                    builder.Append("\n");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Adds the cells string to the string builder.
        /// </summary>
        /// <param name="cells">The puzzle board cells.</param>
        /// <param name="builder">The string builder.</param>
        public static void AddString(Array2D<Cell> cells, StringBuilder builder)
        {
            for (int i = 0; i < cells.Rows; i++)
            {
                for (int j = 0; j < cells.Columns; j++)
                {
                    builder.Append(GetCellCharacter(cells[i, j])).Append(' ');
                }

                builder.Append('\n');
            }
        }

        /// <summary>
        /// Returns the character corresponding to the cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <exception cref="ArgumentException">Raised if the cell value is unhandled.</exception>
        private static char GetCellCharacter(Cell cell)
        {
            switch (cell)
            {
                case Cell.None:
                    return '.';
                case Cell.StopBlock:
                case Cell.StopBlock | Cell.Garbage:
                    return '#';
                case Cell.BreakBlock:
                case Cell.BreakBlock | Cell.Garbage:
                    return '%';
                case Cell.PushBlock:
                    return 'o';
                case Cell.Goal:
                    return '$';
                case Cell.Goal | Cell.PushBlock:
                    return '*';
                case Cell.Void:
                    return '!';
                case Cell.BlockVoid:
                    return '+';
                case Cell.PusherVoid:
                    return '@';
                default:
                    throw new ArgumentException($"Unhandled cell type: {cell}.");
            }
        }
    }
}
