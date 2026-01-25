using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class SingleProcessor : SerializationProcessor<float>
    {
        public override void Process(string name, ref float value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
