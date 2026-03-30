using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="Bounds"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class BoundsProcessor : SerializationProcessor<Bounds>
    {
        [DependencyProcessor]
        private ISerializationProcessor<Vector3> _vector3Processor;

        /// <inheritdoc/>
        protected override void Process(ref Bounds value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(Bounds));

            var center = value.center;
            _vector3Processor.Process("Center", ref center, formatter);
            var size = value.size;
            _vector3Processor.Process("Size", ref size, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new Bounds(center, size);
            }
        }
    }
}
