using System;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.SystemBasic)]
    public class ArrayProcessor<T> : SerializationProcessor<T[]>
    {
        [DependencyProcessor]
        private ISerializationProcessor<T> _serializer;

        protected override void Process(string name, ref T[] value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.BeginObject();

            if (formatter.Operation == FormatterOperation.Write)
            {
                var sizeTag = new SizeTag(value == null ? 0u : (uint)value.Length);
                formatter.Format(ref sizeTag);

                if (value == null)
                    return;

                foreach (var item in value)
                {
                    var tmp = item;
                    _serializer.Process(ref tmp, formatter);
                }
            }
            else
            {
                var sizeTag = new SizeTag();
                formatter.Format(ref sizeTag);

                // Empty array (either was null or empty)
                if (sizeTag.Size == 0)
                {
                    value = Array.Empty<T>();
                    return;
                }

                var total = new T[sizeTag.Size];
                for (int i = 0; i < sizeTag.Size; i++)
                {
                    T item = default;
                    _serializer.Process(ref item, formatter);
                    total[i] = item;
                }

                value = total;
            }
            formatter.EndObject();
            formatter.EndMember();
        }
    }
}
