using System;
using EasyToolKit.Core.Pooling;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Default formatter factory implementation.
    /// Creates formatter instances based on type and buffer capacity.
    /// </summary>
    public sealed class FormatterFactory : IFormatterFactory
    {
        /// <inheritdoc />
        public IReadingFormatter GetReader(SerializationFormat type)
        {
            return type switch
            {
                SerializationFormat.Binary => PoolUtility.RentObject<BinaryReadingFormatter>(),
                _ => throw new ArgumentException($"Unsupported formatter type: {type}")
            };
        }

        /// <inheritdoc />
        public IWritingFormatter GetWriter(SerializationFormat type)
        {
            return type switch
            {
                SerializationFormat.Binary => PoolUtility.RentObject<BinaryWritingFormatter>(),
                _ => throw new ArgumentException($"Unsupported formatter type: {type}")
            };
        }
    }
}
