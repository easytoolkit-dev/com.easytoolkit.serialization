using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of decimal values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class DecimalProcessor : SerializationProcessor<decimal>
    {
        protected override void Process(ref decimal value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
