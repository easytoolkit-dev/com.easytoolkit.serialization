namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Defines tag bytes used in binary serialization format to mark structure boundaries.
    /// </summary>
    internal enum BinaryFormatterTag : byte
    {
        /// <summary>
        /// Marks the beginning of a member with an empty name.
        /// </summary>
        MemberBegin = 0x80,

        /// <summary>
        /// Marks the beginning of a member with a name following the tag.
        /// </summary>
        NamedMemberBegin = 0x81,

        /// <summary>
        /// Marks the beginning of an object.
        /// </summary>
        ObjectBegin = 0x82,

        /// <summary>
        /// Marks the end of an object.
        /// </summary>
        ObjectEnd = 0x83,

        /// <summary>
        /// Marks the beginning of an array.
        /// </summary>
        ArrayBegin = 0x84,

        /// <summary>
        /// Marks the end of an array.
        /// </summary>
        ArrayEnd = 0x85,
    }
}
