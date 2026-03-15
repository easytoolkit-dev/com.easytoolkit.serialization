using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class Int32ArrayProcessor : SerializationProcessor<int[]>
    {
        protected override void Process(ref int[] value, IDataFormatter formatter)
        {
            formatter.Format(ref value);
        }
    }
}
