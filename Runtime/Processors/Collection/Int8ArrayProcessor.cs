using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class Int8ArrayProcessor : SerializationProcessor<sbyte[]>
    {
        protected override void Process(ref sbyte[] value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
