using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class Int8ArrayProcessor : SerializationProcessor<sbyte[]>
    {
        protected override void Process(string name, ref sbyte[] value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
