using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class UInt8ArrayProcessor : SerializationProcessor<byte[]>
    {
        protected override void Process(ref byte[] value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
