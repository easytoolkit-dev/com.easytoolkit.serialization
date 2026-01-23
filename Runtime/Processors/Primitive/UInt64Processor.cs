namespace EasyToolKit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of unsigned 64-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class UInt64Processor : SerializationProcessor<ulong>
    {
        public override void Process(string name, ref ulong value, IDataFormatter formatter)
        {
            using var memberScope = formatter.EnterMember(name);
            formatter.Format(ref value);
        }
    }
}
