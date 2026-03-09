using System;
using System.Collections.Generic;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Collection)]
    public class DictionaryProcessor<TDictionary, TKey, TValue> : SerializationProcessor<TDictionary>
        where TDictionary : class, IDictionary<TKey, TValue>, new()
    {
        [DependencyProcessor]
        private ISerializationProcessor<KeyValuePair<TKey, TValue>> _keyValuePairProcessor;

        protected override void Process(string name, ref TDictionary value, IDataFormatter formatter)
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

                value = new TDictionary();
                for (int i = 0; i < size; i++)
                {
                    var item = new KeyValuePair<TKey, TValue>();
                    _keyValuePairProcessor.Process(ref item, formatter);
                    if (item.Key == null)
                    {
                        throw new SerializationException(
                            "Encountered null key while deserializing dictionary. " +
                            "Dictionary keys cannot be null. " +
                            "Check the serialized data source for integrity.");
                    }

                    value.Add(item);
                }
            }
        }
    }
}
