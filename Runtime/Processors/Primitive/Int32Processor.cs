using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of signed 32-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class Int32Processor : SerializationProcessor<int>
    {
        public override void Process(string name, ref int value, IDataFormatter formatter)
        {
            if (!IsRoot) formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
