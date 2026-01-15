namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.Primitive)]
    public class StringSerializer : EasySerializer<string>
    {
        public override void Process(string name, ref string value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
            formatter.EndMember();
        }
    }
}
