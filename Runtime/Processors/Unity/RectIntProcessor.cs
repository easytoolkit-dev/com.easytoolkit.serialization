using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="RectInt"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class RectIntProcessor : SerializationProcessor<RectInt>
    {
        [DependencyProcessor]
        private ISerializationProcessor<int> _intProcessor;

        /// <inheritdoc/>
        protected override void Process(ref RectInt value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(RectInt));

            var x = value.x;
            _intProcessor.Process("X", ref x, formatter);
            var y = value.y;
            _intProcessor.Process("Y", ref y, formatter);
            var width = value.width;
            _intProcessor.Process("Width", ref width, formatter);
            var height = value.height;
            _intProcessor.Process("Height", ref height, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new RectInt(x, y, width, height);
            }
        }
    }
}
