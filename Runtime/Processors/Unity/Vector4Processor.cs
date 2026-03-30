using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="Vector4"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class Vector4Processor : SerializationProcessor<Vector4>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatProcessor;

        /// <inheritdoc/>
        protected override void Process(ref Vector4 value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(Vector4));

            var x = value.x;
            _floatProcessor.Process("X", ref x, formatter);
            var y = value.y;
            _floatProcessor.Process("Y", ref y, formatter);
            var z = value.z;
            _floatProcessor.Process("Z", ref z, formatter);
            var w = value.w;
            _floatProcessor.Process("W", ref w, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new Vector4(x, y, z, w);
            }
        }
    }
}
