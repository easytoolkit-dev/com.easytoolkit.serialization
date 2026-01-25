using System.Collections.Generic;
using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection - 1, AllowTypeArgumentInheritance = true)]
    public class IListProcessor<T> : SerializationProcessor<IList<T>>
    {
        [DependencyProcessor]
        private ISerializationProcessor<T> _serializer;

        public override void Process(string name, ref IList<T> value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);

            int size;
            if (formatter.Operation == FormatterOperation.Write)
            {
                size = value?.Count ?? 0;
            }
            else
            {
                size = 0;
            }

            using var arrayScope = formatter.EnterArray(ref size);

            if (formatter.Operation == FormatterOperation.Write)
            {
                if (value == null)
                {
                    return;
                }

                var count = value.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = value[i];
                    _serializer.Process(ref item, formatter);
                }
            }
            else
            {
                // Empty list (either was null or empty, keep as null)
                if (size == 0)
                {
                    value = null;
                    return;
                }

                value = new List<T>(size);
                for (int i = 0; i < size; i++)
                {
                    T item = default;
                    _serializer.Process(ref item, formatter);
                    value.Add(item);
                }
            }
        }
    }
}
