using System;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.SystemBasic + 1)]
    public class ArrayProcessor<T> : SerializationProcessor<T[]>
    {
        [DependencyProcessor]
        private ISerializationProcessor<T> _serializer;

        public override void Process(string name, ref T[] value, IDataFormatter formatter)
        {
            if (!IsRoot) formatter.BeginMember(name);

            int size;
            if (formatter.Operation == FormatterOperation.Write)
            {
                size = value == null ? 0 : value.Length;
            }
            else
            {
                size = 0;
            }

            using var arrayScope = formatter.EnterArray(ref size);

            if (formatter.Operation == FormatterOperation.Write)
            {
                if (value == null)
                    return;

                for (var i = 0; i < value.Length; i++)
                {
                    var item = value[i];
                    _serializer.Process(ref item, formatter);
                }
            }
            else
            {
                // Empty array (either was null or empty)
                if (size == 0)
                {
                    value = Array.Empty<T>();
                    return;
                }

                var total = new T[size];
                for (int i = 0; i < size; i++)
                {
                    T item = default;
                    _serializer.Process(ref item, formatter);
                    total[i] = item;
                }

                value = total;
            }
        }
    }
}
