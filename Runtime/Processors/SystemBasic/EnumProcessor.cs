using System;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.SystemBasic)]
    public class EnumProcessor<T> : SerializationProcessor<T>
        where T : struct, Enum
    {
        protected override void Process(string name, ref T value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);

            var direction = formatter.Direction;

            if (formatter.Type != FormatterType.Binary)
            {
                var str = string.Empty;
                if (direction == FormatterDirection.Output)
                    str = Enum.GetName(typeof(T), value);
                formatter.Format(ref str);
                if (direction == FormatterDirection.Input)
                    value = Enum.Parse<T>(str);
            }
            else
            {
                int val = 0;
                if (direction == FormatterDirection.Output)
                    val = Convert.ToInt32(value);
                formatter.Format(ref val);
                if (direction == FormatterDirection.Input)
                    value = (T)(object)val;
            }
            formatter.EndMember();
        }
    }
}
