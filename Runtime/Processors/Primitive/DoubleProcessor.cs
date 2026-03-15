using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class DoubleProcessor : SerializationProcessor<double>
    {
        protected override void Process(ref double value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
