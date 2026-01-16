namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.Primitive)]
    public class DoubleProcessor : SerializationProcessor<double>
    {
        protected override void Process(string name, ref double value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
            formatter.EndMember();
        }
    }
}
