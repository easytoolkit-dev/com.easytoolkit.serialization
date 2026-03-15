using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class Int16ArrayProcessor : SerializationProcessor<short[]>
    {
        protected override void Process(ref short[] value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
