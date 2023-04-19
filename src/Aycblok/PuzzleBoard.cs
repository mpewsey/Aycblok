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
        /// The character to tile dictionary.
        /// </summary>
        private static Dictionary<char, PuzzleTile> CharacterToTileDictionary { get; } = DefaultCharacterToTileDictionary();

        /// <summary>
        /// The tile to character dictionary.
        /// </summary>
        private static Dictionary<PuzzleTile, char> TileToCharacterDictionary { get; } = DefaultTileToCharacterDictionary();

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
        public static string TilesToString(Array2D<PuzzleTile> tiles)
        {
            var size = 2 * tiles.Array.Length + tiles.Rows;
            var builder = new StringBuilder(size);
            AppendTilesToString(tiles, builder);
            return builder.ToString();
        }

        /// <summary>
        /// Returns the puzzle board strings in a tiled layout.
        /// </summary>
        /// <param name="tiles">A list of move puzzle boards.</param>
        /// <param name="columns">The number of columns in the layout.</param>
        /// <param name="tileToCharacter">A function converting a tile to a character. If null, the default delegate will be used.</param>
        public static string TilesToTiledString(IList<Array2D<PuzzleTile>> tiles, int columns, Func<PuzzleTile, char> tileToCharacter = null)
        {
            if (tiles.Count == 0)
                return string.Empty;

            tileToCharacter = tileToCharacter ?? TileToCharacter;
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
                            builder.Append(tileToCharacter.Invoke(tiles[index][i, j])).Append(' ');
                        }

                        // Add separator
                        if (column < columns - 1)
                            builder.Append(' ', spaceCount);
                    }

                    builder.Append('\n');
                }

                if (row < rows - 1)
                    builder.Append('\n');
            }

            return builder.ToString();
        }

        /// <summary>
        /// Adds the tiles string to the string builder.
        /// </summary>
        /// <param name="tiles">The puzzle board tiles.</param>
        /// <param name="builder">The string builder.</param>
        /// <param name="tileToCharacter">A function converting a tile to a character. If null, the default delegate will be used.</param>
        public static void AppendTilesToString(Array2D<PuzzleTile> tiles, StringBuilder builder, Func<PuzzleTile, char> tileToCharacter = null)
        {
            tileToCharacter = tileToCharacter ?? TileToCharacter;

            for (int i = 0; i < tiles.Rows; i++)
            {
                for (int j = 0; j < tiles.Columns; j++)
                {
                    builder.Append(tileToCharacter.Invoke(tiles[i, j])).Append(' ');
                }

                builder.Append('\n');
            }
        }

        /// <summary>
        /// Returns an array of puzzle tiles from a list of puzzle board line strings.
        /// </summary>
        /// <param name="lines">A list of puzzle board line strings.</param>
        /// <param name="characterToTile">A function converting a character to a tile. If null, the default delegate will be used.</param>
        /// <exception cref="ArgumentException">Raised if the puzzle board line strings do not all have equal length.</exception>
        public static Array2D<PuzzleTile> StringsToTiles(IList<string> lines, Func<char, PuzzleTile> characterToTile = null)
        {
            if (lines.Count == 0 || lines[0].Length == 0)
                return new Array2D<PuzzleTile>();

            characterToTile = characterToTile ?? CharacterToTile;
            var result = new Array2D<PuzzleTile>(lines.Count, lines[0].Length);

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (line.Length != lines[0].Length)
                    throw new ArgumentException($"Length of line {i} not equal. Got {line.Length} but expected {lines[0].Length}.");

                for (int j = 0; j < line.Length; j++)
                {
                    result[i, j] = characterToTile.Invoke(line[j]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a new copy of the default character to tile dictionary.
        /// </summary>
        public static Dictionary<char, PuzzleTile> DefaultCharacterToTileDictionary()
        {
            return new Dictionary<char, PuzzleTile>
            {
                { '.', PuzzleTile.None },
                { '#', PuzzleTile.StopBlock },
                { '%', PuzzleTile.BreakBlock },
                { 'o', PuzzleTile.PushBlock },
                { '$', PuzzleTile.Goal },
                { '*', PuzzleTile.Goal | PuzzleTile.PushBlock },
                { '!', PuzzleTile.Void },
                { '+', PuzzleTile.BlockVoid },
                { '@', PuzzleTile.PusherVoid },
            };
        }

        /// <summary>
        /// Returns a new copy of the default tile to character dictionary.
        /// </summary>
        public static Dictionary<PuzzleTile, char> DefaultTileToCharacterDictionary()
        {
            var result = new Dictionary<PuzzleTile, char>(CharacterToTileDictionary.Count);

            foreach (var pair in CharacterToTileDictionary)
            {
                result.Add(pair.Value, pair.Key);
            }

            return result;
        }

        /// <summary>
        /// Returns the puzzle tile corresponding to the specified character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <exception cref="ArgumentException">Raised for an unhandled character.</exception>
        public static PuzzleTile CharacterToTile(char character)
        {
            return CharacterToTileDictionary[character];
        }

        /// <summary>
        /// Returns the character corresponding to the tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <exception cref="ArgumentException">Raised if the tile value is unhandled.</exception>
        public static char TileToCharacter(PuzzleTile tile)
        {
            var layers = PuzzleTile.StopBlock | PuzzleTile.BreakBlock | PuzzleTile.PushBlock | PuzzleTile.Goal | PuzzleTile.Void;
            return TileToCharacterDictionary[tile & layers];
        }
    }
}
