using System;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive - 0.1)]
    public class GenericPrimitiveProcessor<T> : SerializationProcessor<T>
        where T : unmanaged
    {
        private static readonly Type[] ExcludedTypes = { typeof(GenericPrimitiveProcessor<T>) };

        [DependencyProcessor(ExcludedTypesGetter = nameof(ExcludedTypes))]
        private ISerializationProcessor<T> _genericProcessor;

        public override bool CanProcess(Type valueType, SerializationContext context)
        {
            return !valueType.IsGenericType;
        }

        protected override void Process(ref T value, IDataFormatter formatter)
        {
            if (formatter.FormatType == SerializationFormat.Binary)
            {
                var type = typeof(T);
                using var scope = formatter.EnterObject(ref type);
                formatter.FormatGenericPrimitive(ref value);
            }
            else
            {
                _genericProcessor.Process(ref value, formatter);
            }
        }
    }
}
