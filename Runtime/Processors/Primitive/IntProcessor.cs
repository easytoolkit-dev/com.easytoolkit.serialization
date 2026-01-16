namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.Primitive)]
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
