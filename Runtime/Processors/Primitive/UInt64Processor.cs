using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    /// <summary>
    /// Handles serialization and deserialization of unsigned 64-bit integer values.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class UInt64Processor : SerializationProcessor<ulong>
    {
        protected override void Process(string name, ref ulong value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.Format(ref value);
        }
    }
}
