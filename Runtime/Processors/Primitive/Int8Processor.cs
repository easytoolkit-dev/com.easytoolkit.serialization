using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of signed 8-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class Int8Processor : SerializationProcessor<sbyte>
    {
        protected override void Process(ref sbyte value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
