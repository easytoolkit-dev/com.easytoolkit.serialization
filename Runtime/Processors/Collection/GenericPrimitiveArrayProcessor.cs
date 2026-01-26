using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for arrays of generic primitive types (unmanaged types).
    /// Uses optimized binary serialization when available, falling back to generic processor for other formats.
    /// </summary>
    /// <typeparam name="T">The unmanaged element type of the array.</typeparam>
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection + 1)]
    public class GenericPrimitiveArrayProcessor<T> : SerializationProcessor<T[]>
        where T : unmanaged
    {
        private ISerializationProcessor<T[]> _genericProcessor;

        /// <inheritdoc />
        protected override void Process(string name, ref T[] data, IDataFormatter formatter)
        {
            if (formatter.FormatType == SerializationFormat.Binary)
            {
                formatter.BeginMember(name);
                using var scope = formatter.EnterObject(typeof(T));
                formatter.BeginMember("_");
                formatter.FormatGenericPrimitive(ref data);
            }
            else
            {
                _genericProcessor ??= new GenericProcessor<T[]>();
                _genericProcessor.Process(name, ref data, formatter);
            }
        }
    }
}
