using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class Int64ArrayProcessor : SerializationProcessor<long[]>
    {
        protected override void Process(ref long[] value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
