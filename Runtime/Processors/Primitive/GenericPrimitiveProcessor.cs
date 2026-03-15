using System;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive - 0.1)]
    public class GenericPrimitiveProcessor<T> : SerializationProcessor<T>
        where T : unmanaged
    {
        private static readonly Type[] CandidateTypes = { typeof(GenericProcessor<T>) };

        [DependencyProcessor(CandidateTypesGetter = nameof(CandidateTypes))]
        private ISerializationProcessor<T> _genericProcessor;

        public override bool CanProcess(Type valueType)
        {
            return !valueType.IsGenericType;
        }

        protected override void Process(ref T value, IDataFormatter formatter)
        {
            if (formatter.FormatType == SerializationFormat.Binary)
            {
                using var scope = formatter.EnterObject(typeof(T));
                formatter.BeginMember("_");
                formatter.FormatGenericPrimitive(ref value);
            }
            else
            {
                _genericProcessor.Process(ref value, formatter);
            }
        }
    }
}
