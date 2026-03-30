using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="BoundsInt"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class BoundsIntProcessor : SerializationProcessor<BoundsInt>
    {
        [DependencyProcessor]
        private ISerializationProcessor<Vector3Int> _vector3IntProcessor;

        /// <inheritdoc/>
        protected override void Process(ref BoundsInt value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(BoundsInt));

            var position = value.position;
            _vector3IntProcessor.Process("Position", ref position, formatter);
            var size = value.size;
            _vector3IntProcessor.Process("Size", ref size, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new BoundsInt(position, size);
            }
        }
    }
}
