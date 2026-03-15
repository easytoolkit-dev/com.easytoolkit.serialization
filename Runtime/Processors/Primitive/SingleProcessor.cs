using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class SingleProcessor : SerializationProcessor<float>
    {
        protected override void Process(ref float value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
