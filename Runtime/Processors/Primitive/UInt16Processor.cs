namespace EasyToolKit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of unsigned 16-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class UInt16Processor : SerializationProcessor<ushort>
    {
        public override void Process(string name, ref ushort value, IDataFormatter formatter)
        {
            using var memberScope = formatter.EnterMember(name);
            formatter.Format(ref value);
        }
    }
}
