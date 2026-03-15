using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class UInt64ArrayProcessor : SerializationProcessor<ulong[]>
    {
        protected override void Process(ref ulong[] value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
