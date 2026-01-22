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
            formatter.BeginMember(name);
            formatter.BeginObject();

            if (formatter.Operation == FormatterOperation.Write)
            {
                var sizeTag = new SizeTag(value == null ? 0u : (uint)value.Count);
                formatter.Format(ref sizeTag);

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
                var sizeTag = new SizeTag();
                formatter.Format(ref sizeTag);

                // Empty list (either was null or empty, keep as null)
                if (sizeTag.Size == 0)
                {
                    value = null;
                    return;
                }

                value = new List<T>((int)sizeTag.Size);
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
