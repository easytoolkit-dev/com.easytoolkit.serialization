using System;
using System.IO;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Default formatter factory implementation.
    /// Creates formatter instances based on type and stream.
    /// </summary>
    public sealed class FormatterFactory : IFormatterFactory
    {
        /// <inheritdoc />
        public IReadingFormatter CreateReader(SerializationFormat type, Stream input)
        {
            return type switch
            {
                SerializationFormat.Binary => new BinaryReadingFormatter(input),
                SerializationFormat.Json => new JsonReadingFormatter(input),
                _ => throw new ArgumentException($"Unsupported formatter type: {type}")
            };
        }

        /// <inheritdoc />
        public IWritingFormatter CreateWriter(SerializationFormat type, Stream output)
        {
            return type switch
            {
                SerializationFormat.Binary => new BinaryWritingFormatter(output),
                SerializationFormat.Json => new JsonWritingFormatter(output),
                _ => throw new ArgumentException($"Unsupported formatter type: {type}")
            };
        }
    }
}
