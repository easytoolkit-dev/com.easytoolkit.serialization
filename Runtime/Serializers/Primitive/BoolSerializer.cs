namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.Primitive)]
    public class BoolSerializer : EasySerializer<bool>
    {
        public override void Process(string name, ref bool value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
            formatter.EndMember();
        }
    }
}
