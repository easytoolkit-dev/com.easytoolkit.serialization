namespace EasyToolKit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of signed 16-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class Int16Processor : SerializationProcessor<short>
    {
        public override void Process(string name, ref short value, IDataFormatter formatter)
        {
            using var memberScope = formatter.EnterMember(name);
            formatter.Format(ref value);
        }
    }
}
