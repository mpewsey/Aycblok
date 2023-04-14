using System.Runtime.Serialization;

namespace MPewsey.Aycblok
{
    /// <summary>
    /// The layers of a tile in a PuzzleLayout.
    /// </summary>
    [System.Flags]
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public enum PuzzleTile
    {
        /// No tile contents.
        [EnumMember] None = 0,
        /// Tile contains a stop block.
        [EnumMember] StopBlock = 1 << 0,
        /// Tile contains a break block.
        [EnumMember] BreakBlock = 1 << 1,
        /// Tile contains a push block.
        [EnumMember] PushBlock = 1 << 2,
        /// Tile contains a push block goal.
        [EnumMember] Goal = 1 << 3,
        /// Tile does not permit any blocks or the pusher to traverse it.
        [EnumMember] Void = BlockVoid | PusherVoid,
        /// Tile does not permit any blocks to traverse it.
        [EnumMember] BlockVoid = 1 << 4,
        /// Tile does not permit the pusher to traverse it.
        [EnumMember] PusherVoid = 1 << 5,
        /// Tile is out of bounds.
        [EnumMember] OutOfBounds = 1 << 6,
        /// Tile contains a garbage block.
        [EnumMember] Garbage = 1 << 7,
        /// Tile is marked. This is used internally by some generators for tracking but should otherwise not be used.
        [EnumMember] Marked = 1 << 8,
        /// Custom tile marking available for the user.
        [EnumMember] Custom1 = 1 << 9,
        /// Custom tile marking available for the user.
        [EnumMember] Custom2 = 1 << 10,
        /// Custom tile marking available for the user.
        [EnumMember] Custom3 = 1 << 11,
        /// Custom tile marking available for the user.
        [EnumMember] Custom4 = 1 << 12,
        /// Custom tile marking available for the user.
        [EnumMember] Custom5 = 1 << 13,
    }
}
