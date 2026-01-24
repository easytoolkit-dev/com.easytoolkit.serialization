namespace EasyToolKit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of unsigned 32-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class UInt32Processor : SerializationProcessor<uint>
    {
        public override void Process(string name, ref uint value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
