using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 0.2)]
    public class BoolArrayProcessor : SerializationProcessor<bool[]>
    {
        protected override void Process(string name, ref bool[] value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
