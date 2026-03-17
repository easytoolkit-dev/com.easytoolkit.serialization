using System;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Generic)]
    public class NullableProcessor<T> : SerializationProcessor<T?> where T : struct
    {
        [DependencyProcessor]
        private ISerializationProcessor<T> _processor;

        protected override void Process(ref T? value, IDataFormatter formatter)
        {
            var isNull = value == null;
            if (!typeof(T).IsValueType)
            {
                formatter.FormatNullable(ref isNull);
            }
            else
            {
                isNull = false;
            }

            if (!isNull)
            {
                var val = value ?? default;
                _processor.Process(ref val, formatter);
                value = val;
            }
            else
            {
                value = null;
            }
        }
    }
}
