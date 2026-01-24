namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.SystemBasic + 2)]
    public class ByteArrayProcessor : SerializationProcessor<byte[]>
    {
        public override void Process(string name, ref byte[] value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
