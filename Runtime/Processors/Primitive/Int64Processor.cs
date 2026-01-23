namespace EasyToolKit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of signed 64-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class Int64Processor : SerializationProcessor<long>
    {
        public override void Process(string name, ref long value, IDataFormatter formatter)
        {
            using var memberScope = formatter.EnterMember(name);
            formatter.Format(ref value);
        }
    }
}
