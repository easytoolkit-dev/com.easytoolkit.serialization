using System;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for arrays of generic primitive types (unmanaged types).
    /// Uses optimized binary serialization when available, falling back to generic processor for other formats.
    /// </summary>
    /// <typeparam name="T">The unmanaged element type of the array.</typeparam>
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.1)]
    public class GenericPrimitiveArrayProcessor<T> : SerializationProcessor<T[]>
        where T : unmanaged
    {
        private static readonly Type[] ExcludedTypes = { typeof(GenericPrimitiveArrayProcessor<T>) };

        [DependencyProcessor(ExcludedTypesGetter = nameof(ExcludedTypes))]
        private ISerializationProcessor<T[]> _genericProcessor;

        /// <inheritdoc />
        protected override void Process(ref T[] data, IDataFormatter formatter)
        {
            if (formatter.FormatType == SerializationFormat.Binary)
            {
                var type = typeof(T);
                using var scope = formatter.EnterObject(ref type);
                formatter.FormatGenericPrimitive(ref data);
            }
            else
            {
                _genericProcessor.Process(ref data, formatter);
            }
        }
    }
}
