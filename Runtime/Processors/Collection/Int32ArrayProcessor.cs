using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 2)]
    public class Int32ArrayProcessor : SerializationProcessor<int[]>
    {
        public override void Process(string name, ref int[] value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
