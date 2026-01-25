using System;
using EasyToolKit.Core.Pooling;

namespace EasyToolKit.Serialization.Formatters
{
    public static class FormatterFactory
    {
        public static IReadingFormatter GetReader(SerializationFormat type)
        {
            return type switch
            {
                SerializationFormat.Binary => PoolUtility.RentObject<Implementations.BinaryReadingFormatter>(),
                _ => throw new ArgumentException($"Unsupported formatter type: {type}")
            };
        }

        public static IWritingFormatter GetWriter(SerializationFormat type)
        {
            return type switch
            {
                SerializationFormat.Binary => PoolUtility.RentObject<Implementations.BinaryWritingFormatter>(),
                _ => throw new ArgumentException($"Unsupported formatter type: {type}")
            };
        }
    }
}
