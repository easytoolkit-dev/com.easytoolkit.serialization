using System;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Default formatter factory implementation.
    /// Creates formatter instances based on type and buffer capacity.
    /// </summary>
    public sealed class FormatterFactory : IFormatterFactory
    {
        /// <summary>
        /// Gets the default initial capacity for writing formatters.
        /// </summary>
        private const int DefaultInitialCapacity = 1024;

        /// <inheritdoc />
        public IReadingFormatter CreateReader(SerializationFormat type, ReadOnlySpan<byte> buffer)
        {
            return type switch
            {
                SerializationFormat.Binary => new BinaryReadingFormatter(),
                _ => throw new ArgumentException($"Unsupported formatter type: {type}")
            };
        }

        /// <inheritdoc />
        public IWritingFormatter CreateWriter(SerializationFormat type, int initialCapacity = DefaultInitialCapacity)
        {
            return type switch
            {
                SerializationFormat.Binary => new BinaryWritingFormatter(initialCapacity),
                _ => throw new ArgumentException($"Unsupported formatter type: {type}")
            };
        }
    }
}
