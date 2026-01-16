using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.SystemBasic)]
    public class ArraySerializer<T> : EasySerializer<T[]>
    {
        private static readonly EasySerializer<T> Serializer;

        public override void Process(string name, ref T[] value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            formatter.BeginObject();

            var sizeTag = new SizeTag(value == null ? 0 : (uint)value.Length);
            formatter.Format(ref sizeTag);

            if (formatter.Direction == FormatterDirection.Output)
            {
                if (value == null)
                    return;

                foreach (var item in value)
                {
                    var tmp = item;
                    Serializer.Process(ref tmp, formatter);
                }
            }
            else
            {
                var total = new T[sizeTag.Size];
                for (int i = 0; i < sizeTag.Size; i++)
                {
                    T item = default;
                    Serializer.Process(ref item, formatter);
                    total[i] = item;
                }

                value = total;
            }
            formatter.EndObject();
            formatter.EndMember();
        }
    }
}
