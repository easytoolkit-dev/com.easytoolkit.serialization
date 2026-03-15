using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of unsigned 64-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class UInt64Processor : SerializationProcessor<ulong>
    {
        protected override void Process(ref ulong value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
