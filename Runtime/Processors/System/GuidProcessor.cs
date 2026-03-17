using System;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="Guid"/> values.
    /// </summary>
    /// <remarks>
    /// Converts Guid to/from string representation for serialization.
    /// </remarks>
    [ProcessorConfiguration(ProcessorPriorityLevel.System)]
    public class GuidProcessor : SerializationProcessor<Guid>
    {
        protected override void Process(ref Guid value, IDataFormatter formatter)
        {
            var guidString = formatter.Operation == FormatterOperation.Write
                ? value.ToString("D")
                : null;

            formatter.Format(ref guidString);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = string.IsNullOrEmpty(guidString) ? Guid.Empty : Guid.Parse(guidString);
            }
        }
    }
}
