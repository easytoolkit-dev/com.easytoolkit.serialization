using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="Color"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class ColorProcessor : SerializationProcessor<Color>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatProcessor;

        /// <inheritdoc/>
        protected override void Process(ref Color value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(Color));

            var r = value.r;
            _floatProcessor.Process("R", ref r, formatter);
            var g = value.g;
            _floatProcessor.Process("G", ref g, formatter);
            var b = value.b;
            _floatProcessor.Process("B", ref b, formatter);
            var a = value.a;
            _floatProcessor.Process("A", ref a, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new Color(r, g, b, a);
            }
        }
    }
}
