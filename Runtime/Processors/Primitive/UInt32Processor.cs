using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of unsigned 32-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class UInt32Processor : SerializationProcessor<uint>
    {
        protected override void Process(ref uint value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
