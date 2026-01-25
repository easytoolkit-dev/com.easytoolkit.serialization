using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 2)]
    public class UInt16ArrayProcessor : SerializationProcessor<ushort[]>
    {
        public override void Process(string name, ref ushort[] value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
