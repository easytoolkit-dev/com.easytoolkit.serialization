namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class IntProcessor : SerializationProcessor<int>
    {
        protected override void Process(string name, ref int value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
            formatter.EndMember();
        }
    }
}
