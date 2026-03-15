using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class StringProcessor : SerializationProcessor<string>
    {
        protected override void Process(ref string value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
