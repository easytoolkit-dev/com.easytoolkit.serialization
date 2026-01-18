using System.Collections.Generic;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.SystemBasic, AllowTypeArgumentInheritance = true)]
    public class IListProcessor<T> : SerializationProcessor<IList<T>>
    {
        [DependencyProcessor]
        private ISerializationProcessor<T> _serializer;

        protected override void Process(string name, ref IList<T> value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.BeginObject();

            var sizeTag = new SizeTag((uint)value.Count);
            formatter.Format(ref sizeTag);

            if (formatter.Operation == FormatterOperation.Write)
            {
                if (value == null)
                    return;

                var count = value.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = value[i];
                    _serializer.Process(ref item, formatter);
                }
            }
            else
            {
                for (int i = 0; i < sizeTag.Size; i++)
                {
                    T item = default;
                    _serializer.Process(ref item, formatter);
                    value.Add(item);
                }
            }

            formatter.EndObject();
            formatter.EndMember();
        }
    }
}
