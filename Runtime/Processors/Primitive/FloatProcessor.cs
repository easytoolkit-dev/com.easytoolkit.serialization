namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class FloatProcessor : SerializationProcessor<float>
    {
        protected override void Process(string name, ref float value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
            formatter.EndMember();
        }
    }
}
