using MPewsey.Common.Collections;
using MPewsey.Common.Mathematics;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MPewsey.Aycblok
{
    /// <summary>
    /// Contains information pertaining to a puzzle layout and its moves to solve.
    /// </summary>
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public class PuzzleLayout
    {
        /// <summary>
        /// The random seed.
        /// </summary>
        [DataMember(Order = 1)]
        public int Seed { get; private set; }

        /// <summary>
        /// A list of initial push block positions.
        /// </summary>
        [DataMember(Order = 2)]
        public List<Vector2DInt> PushBlockPositions { get; private set; } = new List<Vector2DInt>();

        /// <summary>
        /// The puzzle board cells.
        /// </summary>
        [DataMember(Order = 3)]
        public Array2D<Cell> Cells { get; private set; }

        /// <summary>
        /// A list of moves.
        /// </summary>
        [DataMember(Order = 4)]
        public List<PuzzleMove> Moves { get; private set; } = new List<PuzzleMove>();

        /// <summary>
        /// Initializes a new layout.
        /// </summary>
        /// <param name="cells">The puzzle board cells.</param>
        /// <param name="seed">The random seed.</param>
        public PuzzleLayout(Array2D<Cell> cells, int seed)
        {
            Cells = new Array2D<Cell>(cells);
            Seed = seed;
        }

        public override string ToString()
        {
            return $"PuzzleLayout(Seed = {Seed})";
        }

        /// <summary>
        /// Returns true if the position intersects the push paths of any move.
        /// </summary>
        /// <param name="position">The position.</param>
        public bool Intersects(Vector2DInt position)
        {
            foreach (var move in Moves)
            {
                if (move.Intersects(position))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an array of puzzle boards for every move of the layout.
        /// </summary>
        public Array2D<Cell>[] GetPuzzleBoards()
        {
            var result = new Array2D<Cell>[Moves.Count + 1];
            var cells = new Array2D<Cell>(Cells);
            result[0] = cells;

            for (int i = 0; i < Moves.Count; i++)
            {
                cells = new Array2D<Cell>(cells);
                Moves[i].Apply(cells);
                result[i + 1] = cells;
            }

            return result;
        }

        /// <summary>
        /// Returns the puzzle board strings in a tiled layout.
        /// </summary>
        /// <param name="columns">The number of columns in the layout.</param>
        public string GetTiledMoveReport(int columns)
        {
            return PuzzleBoard.GetTiledString(GetPuzzleBoards(), columns);
        }

        /// <summary>
        /// Returns a string with puzzle boards for every move of the layout.
        /// </summary>
        public string GetMoveReport()
        {
            var cells = new Array2D<Cell>(Cells);
            var size = (Moves.Count + 1) * (2 * cells.Array.Length + cells.Rows + 15);
            var builder = new StringBuilder(size);
            builder.Append("Start board:\n");
            PuzzleBoard.AddString(cells, builder);

            for (int i = 0; i < Moves.Count; i++)
            {
                Moves[i].Apply(cells);
                builder.Append("\nMove ").Append(i + 1).Append(":\n");
                PuzzleBoard.AddString(cells, builder);
            }

            return builder.ToString();
        }
    }
}
