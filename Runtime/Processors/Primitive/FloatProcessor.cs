namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class FloatProcessor : SerializationProcessor<float>
    {
        public override void Process(string name, ref float value, IDataFormatter formatter)
        {
            using var memberScope = formatter.EnterMember(name);
            formatter.Format(ref value);
        }
    }
}
