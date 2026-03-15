using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class BoolArrayProcessor : SerializationProcessor<bool[]>
    {
        protected override void Process(ref bool[] value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
