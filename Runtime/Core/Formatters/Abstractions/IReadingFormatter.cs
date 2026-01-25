using System;

namespace EasyToolKit.Serialization.Formatters
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
    }
}
