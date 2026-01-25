using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive - 1)]
    public class GenericPrimitiveProcessor<T> : SerializationProcessor<T>
        where T : unmanaged
    {
        private GenericProcessor<T> _genericProcessor;

        public override void Process(string name, ref T value, IDataFormatter formatter)
        {
            if (formatter.FormatType == SerializationFormat.Binary)
            {
                if (!IsRoot) formatter.BeginMember(name);
                formatter.FormatGenericPrimitive(ref value);
            }
            else
            {
                _genericProcessor ??= new GenericProcessor<T>();
                _genericProcessor.Process(name, ref value, formatter);
            }
        }
    }
}
