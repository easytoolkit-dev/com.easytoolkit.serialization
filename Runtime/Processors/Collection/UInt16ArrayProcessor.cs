using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class UInt16ArrayProcessor : SerializationProcessor<ushort[]>
    {
        protected override void Process(ref ushort[] value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
