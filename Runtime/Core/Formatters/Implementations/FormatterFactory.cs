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
        public IReadingFormatter CreateReader(FormatterType type, Stream input)
        {
            return type switch
            {
                FormatterType.Binary => new BinaryReadingFormatter(input),
                FormatterType.Json => new JsonReadingFormatter(input),
                _ => throw new ArgumentException($"Unsupported formatter type: {type}")
            };
        }

        /// <inheritdoc />
        public IWritingFormatter CreateWriter(FormatterType type, Stream output)
        {
            return type switch
            {
                FormatterType.Binary => new BinaryWritingFormatter(output),
                FormatterType.Json => new JsonWritingFormatter(output),
                _ => throw new ArgumentException($"Unsupported formatter type: {type}")
            };
        }
    }
}
