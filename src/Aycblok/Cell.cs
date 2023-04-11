using System.Runtime.Serialization;

namespace MPewsey.Aycblok
{
    /// <summary>
    /// The layers of a cell in a PuzzleLayout.
    /// </summary>
    [System.Flags]
    [DataContract(Namespace = Constants.DataContractNamespace)]
    public enum Cell
    {
        /// No cell contents.
        [EnumMember] None = 0,
        /// Cell contains a stop block.
        [EnumMember] StopBlock = 1 << 0,
        /// Cell contains a break block.
        [EnumMember] BreakBlock = 1 << 1,
        /// Cell contains a push block.
        [EnumMember] PushBlock = 1 << 2,
        /// Cell contains a push block goal.
        [EnumMember] Goal = 1 << 3,
        /// Cell does not permit any blocks or the pusher to traverse it.
        [EnumMember] Void = BlockVoid | PusherVoid,
        /// Cell does not permit any blocks to traverse it.
        [EnumMember] BlockVoid = 1 << 4,
        /// Cell does not permit the pusher to traverse it.
        [EnumMember] PusherVoid = 1 << 5,
        /// Cell is out of bounds.
        [EnumMember] OutOfBounds = 1 << 6,
        /// Cell contains a garbage block.
        [EnumMember] Garbage = 1 << 7,
        /// Cell is marked. This is used internally by some generators for tracking but should otherwise not be used.
        [EnumMember] Marked = 1 << 8,
    }
}
