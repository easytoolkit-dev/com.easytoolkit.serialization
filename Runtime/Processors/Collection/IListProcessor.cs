using System.Collections.Generic;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.SystemBasic, AllowTypeArgumentInheritance = true)]
    public class IListProcessor<T> : SerializationProcessor<IList<T>>
    {
        [DependencyProcessor]
        private ISerializationProcessor<T> _serializer;

        public override void Process(string name, ref IList<T> value, IDataFormatter formatter)
        {
            using var memberScope = formatter.EnterMember(name);
            using var objectScope = formatter.EnterObject();

            if (formatter.Operation == FormatterOperation.Write)
            {
                var size = value?.Count ?? 0;
                formatter.Format(ref size);

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
                var size = 0;
                formatter.Format(ref size);

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
