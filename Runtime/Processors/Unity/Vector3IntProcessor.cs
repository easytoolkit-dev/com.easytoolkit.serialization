using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="Vector3Int"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class Vector3IntProcessor : SerializationProcessor<Vector3Int>
    {
        [DependencyProcessor]
        private ISerializationProcessor<int> _intProcessor;

        /// <inheritdoc/>
        protected override void Process(ref Vector3Int value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(Vector3Int));

            var x = value.x;
            _intProcessor.Process("X", ref x, formatter);
            var y = value.y;
            _intProcessor.Process("Y", ref y, formatter);
            var z = value.z;
            _intProcessor.Process("Z", ref z, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new Vector3Int(x, y, z);
            }
        }
    }
}
