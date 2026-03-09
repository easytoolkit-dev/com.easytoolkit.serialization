using System.Collections.Generic;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection - 0.1, AllowTypeArgumentInheritance = true)]
    public class ListProcessor<TCollection, TItem> : SerializationProcessor<TCollection>
        where TCollection : class, IList<TItem>, new()
    {
        [DependencyProcessor]
        private ISerializationProcessor<TItem> _itemProcessor;

        protected override void Process(string name, ref TCollection value, IDataFormatter formatter)
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
                    _itemProcessor.Process(ref item, formatter);
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

                value = new TCollection();
                for (int i = 0; i < size; i++)
                {
                    TItem item = default;
                    _itemProcessor.Process(ref item, formatter);
                    value.Add(item);
                }
            }
        }
    }
}
