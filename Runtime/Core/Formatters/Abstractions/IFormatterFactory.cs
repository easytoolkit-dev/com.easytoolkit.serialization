using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Defines the contract for creating formatter instances.
    /// </summary>
    public interface IFormatterFactory
    {
        /// <summary>
        /// Gets a reading formatter for deserialization from a byte buffer.
        /// </summary>
        /// <param name="type">The serialization format type.</param>
        /// <returns>A new reading formatter instance.</returns>
        IReadingFormatter GetReader(SerializationFormat type);

        /// <summary>
        /// Gets a writing formatter for serialization to an internal buffer.
        /// </summary>
        /// <param name="type">The serialization format type.</param>
        /// <returns>A new writing formatter instance.</returns>
        IWritingFormatter GetWriter(SerializationFormat type);
    }
}
