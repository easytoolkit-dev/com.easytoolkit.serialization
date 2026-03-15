using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class BoolProcessor : SerializationProcessor<bool>
    {
        protected override void Process(ref bool value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
