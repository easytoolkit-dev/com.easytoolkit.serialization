using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Defines the contract for creating formatter instances.
    /// </summary>
    public interface IFormatterFactory
    {
        /// <summary>
        /// Creates a reading formatter for deserialization from a byte buffer.
        /// </summary>
        /// <param name="type">The serialization format type.</param>
        /// <param name="buffer">The byte buffer containing the serialized data.</param>
        /// <returns>A new reading formatter instance.</returns>
        IReadingFormatter CreateReader(SerializationFormat type, ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Creates a writing formatter for serialization to an internal buffer.
        /// </summary>
        /// <param name="type">The serialization format type.</param>
        /// <param name="initialCapacity">The initial capacity of the internal buffer in bytes.</param>
        /// <returns>A new writing formatter instance.</returns>
        IWritingFormatter CreateWriter(SerializationFormat type, int initialCapacity = 1024);
    }
}
