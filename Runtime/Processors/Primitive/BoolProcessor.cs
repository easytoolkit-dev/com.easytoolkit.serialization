namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.Primitive)]
    public class BoolProcessor : SerializationProcessor<bool>
    {
        protected override void Process(string name, ref bool value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
            formatter.EndMember();
        }
    }
}
