namespace EasyToolKit.Serialization.Formatters.Implementations
{
    /// <summary>
    /// Defines tag bytes used in binary serialization format to mark structure boundaries and value types.
    /// Value type tags use 0x00-0x3F range, structure markers use 0x80+ range to distinguish them.
    /// </summary>
    internal enum BinaryFormatterTag : byte
    {
        #region Value Type Tags (0x10-0x3F)

        /// <summary>
        /// Tag for 8-bit signed integer (sbyte) values.
        /// </summary>
        Int8 = 0x10,

        /// <summary>
        /// Tag for 16-bit signed integer (short) values.
        /// </summary>
        Int16 = 0x11,

        /// <summary>
        /// Tag for 32-bit signed integer (int) values.
        /// </summary>
        Int32 = 0x12,

        /// <summary>
        /// Tag for 64-bit signed integer (long) values.
        /// </summary>
        Int64 = 0x13,

        /// <summary>
        /// Tag for 8-bit unsigned integer (byte) values.
        /// </summary>
        UInt8 = 0x14,

        /// <summary>
        /// Tag for 16-bit unsigned integer (ushort) values.
        /// </summary>
        UInt16 = 0x15,

        /// <summary>
        /// Tag for 32-bit unsigned integer (uint) values.
        /// </summary>
        UInt32 = 0x16,

        /// <summary>
        /// Tag for 64-bit unsigned integer (ulong) values.
        /// </summary>
        UInt64 = 0x17,

        /// <summary>
        /// Tag for boolean (bool) values.
        /// </summary>
        Boolean = 0x18,

        /// <summary>
        /// Tag for 32-bit floating point (float) values.
        /// </summary>
        Single = 0x19,

        /// <summary>
        /// Tag for 64-bit floating point (double) values.
        /// </summary>
        Double = 0x1A,

        /// <summary>
        /// Tag for string (string) values. Followed by length-prefixed UTF-8 bytes.
        /// </summary>
        String = 0x1B,

        /// <summary>
        /// Tag for byte array (byte[]) values. Followed by length-prefixed bytes.
        /// </summary>
        ByteArray = 0x1C,

        /// <summary>
        /// Tag for sbyte array (sbyte[]) values. Followed by length-prefixed bytes.
        /// </summary>
        SByteArray = 0x1D,

        /// <summary>
        /// Tag for short array (short[]) values. Followed by length-prefixed bytes.
        /// </summary>
        Int16Array = 0x1E,

        /// <summary>
        /// Tag for int array (int[]) values. Followed by length-prefixed bytes.
        /// </summary>
        Int32Array = 0x1F,

        /// <summary>
        /// Tag for long array (long[]) values. Followed by length-prefixed bytes.
        /// </summary>
        Int64Array = 0x20,

        /// <summary>
        /// Tag for ushort array (ushort[]) values. Followed by length-prefixed bytes.
        /// </summary>
        UInt16Array = 0x21,

        /// <summary>
        /// Tag for uint array (uint[]) values. Followed by length-prefixed bytes.
        /// </summary>
        UInt32Array = 0x22,

        /// <summary>
        /// Tag for ulong array (ulong[]) values. Followed by length-prefixed bytes.
        /// </summary>
        UInt64Array = 0x23,

        /// <summary>
        /// Tag for Unity object reference values. Followed by varint32 reference index.
        /// </summary>
        UnityObjectRef = 0x24,

        /// <summary>
        /// Tag for generic unmanaged values (custom structs, enums). Followed by raw bytes.
        /// </summary>
        UnmanagedValue = 0x25,

        /// <summary>
        /// Tag for generic unmanaged array values (custom struct arrays, enum arrays). Followed by length-prefixed raw bytes.
        /// </summary>
        UnmanagedArray = 0x26,

        #endregion

        #region Structure Marker Tags (0x80-0xFF)

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
        /// Marks the beginning of an object with type information.
        /// Followed by the object's type full name string, then the object content.
        /// </summary>
        TypedObjectBegin = 0x85,

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

        #endregion
    }
}
