using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of unsigned 8-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class UInt8Processor : SerializationProcessor<byte>
    {
        protected override void Process(ref byte value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
