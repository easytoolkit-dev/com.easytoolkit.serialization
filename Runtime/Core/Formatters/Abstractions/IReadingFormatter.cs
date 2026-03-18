using System;

namespace EasyToolkit.Serialization.Formatters
{
    /// <summary>
    /// Defines the contract for reading formatters that deserialize data from a byte buffer.
    /// </summary>
    public interface IReadingFormatter : IDataFormatter, IObjectReferenceReader
    {
        /// <summary>
        /// Sets the data buffer to read from. This allows the formatter to be reused in object pools.
        /// </summary>
        /// <param name="buffer">The read-only span of bytes containing the serialized data.</param>
        void SetBuffer(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Gets a read-only view of the underlying data buffer.
        /// </summary>
        /// <returns>A read-only span of bytes representing the current data buffer.</returns>
        ReadOnlySpan<byte> GetBuffer();

        /// <summary>
        /// Gets the current read position in bytes.
        /// </summary>
        /// <returns>The current position in the buffer (in bytes).</returns>
        int GetPosition();

        /// <summary>
        /// Gets the remaining bytes available to read.
        /// </summary>
        /// <returns>The number of bytes remaining from the current position to the end of the buffer.</returns>
        int GetRemainingLength();

        /// <summary>
        /// Peeks at the type information of the next object without advancing the read position.
        /// </summary>
        /// <param name="expectedType">The expected type to validate against. If null, no type validation is performed.</param>
        /// <returns>The type of the next object, or null if type information is not available.</returns>
        /// <remarks>
        /// This method allows previewing the type information before reading the object.
        /// Unlike <see cref="IDataFormatter.BeginObject(System.Type)"/>, this method does not
        /// consume any data or modify the internal position, making it useful for conditional
        /// deserialization based on the actual type in the stream.
        /// </remarks>
        Type PeekType(Type expectedType = null);
    }
}
