using System;
using System.Collections.Generic;
using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection)]
    public class DictionaryProcessor<TKey, TValue> : SerializationProcessor<IDictionary<TKey, TValue>>
    {
        [DependencyProcessor]
        private ISerializationProcessor<KeyValuePair<TKey, TValue>> _keyValuePairProcessor;

        protected override void Process(string name, ref IDictionary<TKey, TValue> value, IDataFormatter formatter)
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

                foreach (var item in value)
                {
                    var refItem = item;
                    _keyValuePairProcessor.Process(ref refItem, formatter);
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

                for (int i = 0; i < size; i++)
                {
                    var item = new KeyValuePair<TKey, TValue>();
                    _keyValuePairProcessor.Process(ref item, formatter);
                    value!.Add(item);
                }
            }
        }
    }
}
