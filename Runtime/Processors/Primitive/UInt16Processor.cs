using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of unsigned 16-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class UInt16Processor : SerializationProcessor<ushort>
    {
        protected override void Process(ref ushort value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
