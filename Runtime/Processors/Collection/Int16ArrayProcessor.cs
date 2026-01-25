using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.SystemBasic + 2)]
    public class Int16ArrayProcessor : SerializationProcessor<short[]>
    {
        public override void Process(string name, ref short[] value, IDataFormatter formatter)
        {
            if (!IsRoot) formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
