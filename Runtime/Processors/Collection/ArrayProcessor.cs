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
            using var memberScope = formatter.EnterMember(name);
            using var objectScope = formatter.EnterObject();

            if (formatter.Operation == FormatterOperation.Write)
            {
                var size = value.Length;
                formatter.Format(ref size);

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
                var size = 0;
                formatter.Format(ref size);

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
