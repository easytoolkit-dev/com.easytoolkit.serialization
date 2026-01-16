using System.Collections.Generic;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(priority: (int)ProcessorPriorityLevel.SystemBasic)]
    public class IListProcessor<T> : SerializationProcessor<IList<T>>
    {
        private static readonly ISerializationProcessor<T> Serializer;

        protected override void Process(string name, ref IList<T> value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.BeginObject();

            var sizeTag = new SizeTag(value == null ? 0 : (uint)value.Count);
            formatter.Format(ref sizeTag);

            if (formatter.Direction == FormatterDirection.Output)
            {
                if (value == null)
                    return;

                var count = value.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = value[i];
                    Serializer.Process(ref item, formatter);
                }
            }
            else
            {
                var total = new List<T>((int)sizeTag.Size);
                for (int i = 0; i < sizeTag.Size; i++)
                {
                    T item = default;
                    Serializer.Process(ref item, formatter);
                    total.Add(item);
                }

                value = total;
            }

            formatter.EndObject();
            formatter.EndMember();
        }
    }
}
