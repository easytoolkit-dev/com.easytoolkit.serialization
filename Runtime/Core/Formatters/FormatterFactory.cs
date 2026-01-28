using System;
using EasyToolkit.Core.Pooling;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Formatters
{
    public static class FormatterFactory
    {
        public static IReadingFormatter GetReader(SerializationFormat type, [NotNull] SerializationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            switch (type)
            {
                case SerializationFormat.Binary:
                {
                    var formatter = PoolUtility.RentObject<Implementations.BinaryReadingFormatter>();
                    formatter.Settings = settings.BinaryFormatterSettings;
                    return formatter;
                }
                default:
                    throw new ArgumentException($"Unsupported formatter type: {type}");
            }
        }

        public static IWritingFormatter GetWriter(SerializationFormat type, [NotNull] SerializationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            switch (type)
            {
                case SerializationFormat.Binary:
                    var formatter = PoolUtility.RentObject<Implementations.BinaryWritingFormatter>();
                    formatter.Settings = settings.BinaryFormatterSettings;
                    return formatter;
                default:
                    throw new ArgumentException($"Unsupported formatter type: {type}");
            }
        }
    }
}
