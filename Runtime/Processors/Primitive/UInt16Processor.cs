using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of unsigned 16-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class UInt16Processor : SerializationProcessor<ushort>
    {
        public override void Process(string name, ref ushort value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
