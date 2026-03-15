using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of signed 32-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class Int32Processor : SerializationProcessor<int>
    {
        protected override void Process(ref int value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
