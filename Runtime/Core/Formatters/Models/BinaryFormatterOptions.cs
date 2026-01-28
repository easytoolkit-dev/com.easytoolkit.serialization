using System;

namespace EasyToolkit.Serialization.Formatters
{
    /// <summary>
    /// Defines optimization options for binary serialization format.
    /// These flags control which formatter features are enabled and can be combined.
    /// </summary>
    [Flags]
    public enum BinaryFormatterOptions
    {
        /// <summary>
        /// No formatter options enabled. All features are disabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enables writing type tag bytes before each value.
        /// When disabled, the deserializer must rely on structure information to determine types.
        /// </summary>
        IncludeTypeTags = 1 << 0,

        /// <summary>
        /// Enables writing member name strings.
        /// When disabled, members are serialized by position only without name information.
        /// </summary>
        IncludeMemberNames = 1 << 1,

        /// <summary>
        /// Enables writing type information before object begin markers.
        /// When enabled, object type full name is serialized for polymorphic deserialization.
        /// When disabled, objects are deserialized using the known type structure.
        /// </summary>
        IncludeObjectType = 1 << 2,

        /// <summary>
        /// Enables varint encoding for integer types.
        /// When disabled, integers are written in fixed-width format (1, 2, 4, or 8 bytes).
        /// Varint uses 1-5 bytes for 32-bit integers and 1-10 bytes for 64-bit integers.
        /// </summary>
        EnableVarintEncoding = 1 << 3,

        /// <summary>
        /// Enables direct memory copy for string serialization (2 bytes per char).
        /// When disabled, strings are encoded using UTF-8 encoding.
        /// Direct memory copy is faster but uses more space for ASCII text.
        /// </summary>
        EnableDirectMemoryCopy = 1 << 4,

        /// <summary>
        /// Default configuration with type tags, member names, object type, and direct memory copy enabled.
        /// Provides a good balance between compatibility, functionality, and performance.
        /// </summary>
        Default = IncludeTypeTags | IncludeMemberNames | IncludeObjectType | EnableDirectMemoryCopy,

        /// <summary>
        /// Maximum performance configuration with only direct memory copy enabled.
        /// Disables type tags and member names to minimize serialization overhead.
        /// Use when performance is critical and structure information is known in advance.
        /// </summary>
        Performance = EnableDirectMemoryCopy,

        /// <summary>
        /// Minimum data size configuration with varint encoding enabled.
        /// Disables type tags, member names, and direct memory copy to minimize serialized data size.
        /// Use when storage/bandwidth is more important than serialization speed.
        /// </summary>
        MinimizeSize = EnableVarintEncoding
    }
}
