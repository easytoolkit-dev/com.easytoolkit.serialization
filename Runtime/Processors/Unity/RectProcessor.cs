using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="Rect"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class RectProcessor : SerializationProcessor<Rect>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatProcessor;

        /// <inheritdoc/>
        protected override void Process(ref Rect value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(Rect));

            var x = value.x;
            _floatProcessor.Process("X", ref x, formatter);
            var y = value.y;
            _floatProcessor.Process("Y", ref y, formatter);
            var width = value.width;
            _floatProcessor.Process("Width", ref width, formatter);
            var height = value.height;
            _floatProcessor.Process("Height", ref height, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new Rect(x, y, width, height);
            }
        }
    }
}
