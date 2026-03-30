using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="Vector3"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class Vector3Processor : SerializationProcessor<Vector3>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatProcessor;

        /// <inheritdoc/>
        protected override void Process(ref Vector3 value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(Vector3));

            var x = value.x;
            _floatProcessor.Process("X", ref x, formatter);
            var y = value.y;
            _floatProcessor.Process("Y", ref y, formatter);
            var z = value.z;
            _floatProcessor.Process("Z", ref z, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new Vector3(x, y, z);
            }
        }
    }
}
