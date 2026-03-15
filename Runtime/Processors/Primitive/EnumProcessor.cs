using System;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Primitive)]
    public class EnumProcessor<T> : SerializationProcessor<T>
        where T : struct, Enum
    {
        protected override void Process(ref T value, IDataFormatter formatter)
        {
            if (formatter.FormatType != SerializationFormat.Binary)
            {
                var str = string.Empty;
                if (formatter.Operation == FormatterOperation.Write)
                    str = Enum.GetName(typeof(T), value);
                formatter.Format(ref str);
                if (formatter.Operation == FormatterOperation.Read)
                    value = Enum.Parse<T>(str);
            }
            else
            {
                int val = 0;
                if (formatter.Operation == FormatterOperation.Write)
                    val = Convert.ToInt32(value);
                formatter.Format(ref val);
                if (formatter.Operation == FormatterOperation.Read)
                    value = (T)(object)val;
            }
        }
    }
}
