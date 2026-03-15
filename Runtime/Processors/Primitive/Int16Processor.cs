using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of signed 16-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class Int16Processor : SerializationProcessor<short>
    {
        protected override void Process(ref short value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
