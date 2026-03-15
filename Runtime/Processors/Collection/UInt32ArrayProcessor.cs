using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class UInt32ArrayProcessor : SerializationProcessor<uint[]>
    {
        protected override void Process(ref uint[] value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
