using System;
using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.SystemBasic)]
    public class EnumProcessor<T> : SerializationProcessor<T>
        where T : struct, Enum
    {
        public override void Process(string name, ref T value, IDataFormatter formatter)
        {
            if (!IsRoot) formatter.BeginMember(name);

            if (formatter.Type != SerializationFormat.Binary)
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
