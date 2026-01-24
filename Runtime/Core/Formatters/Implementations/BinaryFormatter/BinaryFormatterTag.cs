namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Defines tag bytes used in binary serialization format to mark structure boundaries.
    /// </summary>
    internal enum BinaryFormatterTag : byte
    {
        /// <summary>
        /// Marks the beginning of a member. Always followed by a length-prefixed name string.
        /// Anonymous members use auto-generated names like "$0", "$1", etc.
        /// </summary>
        MemberBegin = 0x80,

        /// <summary>
        /// Marks the beginning of an object.
        /// </summary>
        ObjectBegin = 0x81,

        /// <summary>
        /// Marks the end of an object.
        /// </summary>
        ObjectEnd = 0x82,

        /// <summary>
        /// Marks the beginning of an array.
        /// </summary>
        ArrayBegin = 0x83,

        /// <summary>
        /// Marks the end of an array.
        /// </summary>
        ArrayEnd = 0x84,
    }
}
