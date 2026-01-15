namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Variable-length integer encoding for efficient storage of small integers.
    /// Uses 7-bit encoding where each byte contains 7 bits of data and a continuation bit.
    /// </summary>
    public struct Varint32
    {
        /// <summary>The decoded integer value.</summary>
        public uint Value { get; set; }

        /// <summary>Creates a new Varint32 with the specified value.</summary>
        public Varint32(uint value)
        {
            Value = value;
        }
    }
}
