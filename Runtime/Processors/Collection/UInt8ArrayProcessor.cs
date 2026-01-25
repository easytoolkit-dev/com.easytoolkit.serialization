using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 2)]
    public class UInt8ArrayProcessor : SerializationProcessor<byte[]>
    {
        public override void Process(string name, ref byte[] value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
