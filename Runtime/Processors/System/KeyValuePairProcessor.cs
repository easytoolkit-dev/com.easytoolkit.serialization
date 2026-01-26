using System.Collections.Generic;
using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.System)]
    public class KeyValuePairProcessor<TKey, TValue> : SerializationProcessor<KeyValuePair<TKey, TValue>>
    {
        [DependencyProcessor]
        private ISerializationProcessor<TKey> _keyProcessor;

        [DependencyProcessor]
        private ISerializationProcessor<TValue> _valueProcessor;

        protected override void Process(string name, ref KeyValuePair<TKey, TValue> keyValuePair, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            using var scope = formatter.EnterObject(ValueType);

            var key = keyValuePair.Key;
            _keyProcessor.Process("Key", ref key, formatter);
            var value = keyValuePair.Value;
            _valueProcessor.Process("Value", ref value, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                keyValuePair = new KeyValuePair<TKey, TValue>(key, value);
            }
        }
    }
}
