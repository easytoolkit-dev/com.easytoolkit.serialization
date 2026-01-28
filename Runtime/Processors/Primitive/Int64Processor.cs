using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of signed 64-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class Int64Processor : SerializationProcessor<long>
    {
        protected override void Process(string name, ref long value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
