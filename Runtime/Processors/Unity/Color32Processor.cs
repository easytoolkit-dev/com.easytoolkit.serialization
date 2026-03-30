using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="Color32"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class Color32Processor : SerializationProcessor<Color32>
    {
        [DependencyProcessor]
        private ISerializationProcessor<byte> _byteProcessor;

        /// <inheritdoc/>
        protected override void Process(ref Color32 value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(Color32));

            var r = value.r;
            _byteProcessor.Process("R", ref r, formatter);
            var g = value.g;
            _byteProcessor.Process("G", ref g, formatter);
            var b = value.b;
            _byteProcessor.Process("B", ref b, formatter);
            var a = value.a;
            _byteProcessor.Process("A", ref a, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new Color32(r, g, b, a);
            }
        }
    }
}
