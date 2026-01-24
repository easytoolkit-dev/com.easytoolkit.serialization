namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Defines the contract for writing formatters that serialize data to a byte buffer.
    /// </summary>
    public interface IWritingFormatter : IDataFormatter, IObjectReferenceWriter
    {
        /// <summary>
        /// Gets the internal buffer array (direct reference, not a copy).
        /// This provides access to the underlying buffer for direct manipulation.
        /// </summary>
        /// <returns>The internal byte array buffer.</returns>
        byte[] GetBuffer();

        /// <summary>
        /// Gets the current write position in bytes.
        /// </summary>
        /// <returns>The current position in the buffer (in bytes).</returns>
        int GetPosition();

        /// <summary>
        /// Gets the current data length (written bytes count).
        /// </summary>
        /// <returns>The number of bytes written to the buffer.</returns>
        int GetLength();

        /// <summary>
        /// Copies written data to a new byte array.
        /// </summary>
        /// <returns>A new byte array containing the serialized data.</returns>
        byte[] ToArray();
    }
}
