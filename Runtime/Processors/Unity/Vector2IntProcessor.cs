using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="Vector2Int"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class Vector2IntProcessor : SerializationProcessor<Vector2Int>
    {
        [DependencyProcessor]
        private ISerializationProcessor<int> _intProcessor;

        /// <inheritdoc/>
        protected override void Process(ref Vector2Int value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(Vector2Int));

            var x = value.x;
            _intProcessor.Process("X", ref x, formatter);
            var y = value.y;
            _intProcessor.Process("Y", ref y, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new Vector2Int(x, y);
            }
        }
    }
}
