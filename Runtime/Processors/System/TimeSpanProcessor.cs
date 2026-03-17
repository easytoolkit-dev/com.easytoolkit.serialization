using System;
using EasyToolkit.Serialization;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="TimeSpan"/> values.
    /// </summary>
    /// <remarks>
    /// Serializes TimeSpan differently based on format:
    /// - Binary format: Uses ticks (long) for optimal performance and precision
    /// - Non-Binary formats (Json/Xml/Yaml): Uses string format for human readability
    /// </remarks>
    [ProcessorConfiguration(ProcessorPriorityLevel.System)]
    public class TimeSpanProcessor : SerializationProcessor<TimeSpan>
    {
        protected override void Process(ref TimeSpan value, IDataFormatter formatter)
        {
            if (formatter.FormatType == SerializationFormat.Binary)
            {
                SerializeAsBinary(ref value, formatter);
            }
            else
            {
                SerializeAsString(ref value, formatter);
            }
        }

        private void SerializeAsBinary(ref TimeSpan value, IDataFormatter formatter)
        {
            var ticks = value.Ticks;
            formatter.Format(ref ticks);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = TimeSpan.FromTicks(ticks);
            }
        }

        private void SerializeAsString(ref TimeSpan value, IDataFormatter formatter)
        {
            var timeSpanString = formatter.Operation == FormatterOperation.Write
                ? value.ToString("g")
                : null;

            formatter.Format(ref timeSpanString);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = string.IsNullOrEmpty(timeSpanString) ? TimeSpan.Zero : TimeSpan.Parse(timeSpanString);
            }
        }
    }
}
