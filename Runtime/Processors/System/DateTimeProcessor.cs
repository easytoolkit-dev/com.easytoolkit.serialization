using System;
using System.Globalization;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="DateTime"/> values.
    /// </summary>
    /// <remarks>
    /// Serializes DateTime differently based on format:
    /// - Binary format: Uses ticks (long) and kind (byte) for optimal performance
    /// - Non-Binary formats (Json/Xml/Yaml): Uses ISO 8601 string format for human readability
    /// </remarks>
    [ProcessorConfiguration(ProcessorPriorityLevel.System)]
    public class DateTimeProcessor : SerializationProcessor<DateTime>
    {
        [DependencyProcessor]
        private ISerializationProcessor<long> _ticksProcessor;
        [DependencyProcessor]
        private ISerializationProcessor<DateTimeKind> _dateTimeKindProcessor;

        protected override void Process(ref DateTime value, IDataFormatter formatter)
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

        private void SerializeAsBinary(ref DateTime value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(ValueType);

            var ticks = value.Ticks;
            _ticksProcessor.Process("Ticks", ref ticks, formatter);

            var kind = value.Kind;
            _dateTimeKindProcessor.Process("Kind", ref kind, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new DateTime(ticks, kind);
            }
        }

        private void SerializeAsString(ref DateTime value, IDataFormatter formatter)
        {
            var dateString = formatter.Operation == FormatterOperation.Write
                ? value.ToString("O")
                : null;

            formatter.Format(ref dateString);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = string.IsNullOrEmpty(dateString) ? default : DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
        }
    }
}
