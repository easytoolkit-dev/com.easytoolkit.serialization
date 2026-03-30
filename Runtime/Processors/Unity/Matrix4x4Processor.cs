using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Serialization processor for <see cref="Matrix4x4"/>.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class Matrix4x4Processor : SerializationProcessor<Matrix4x4>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatProcessor;

        /// <inheritdoc/>
        protected override void Process(ref Matrix4x4 value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(Matrix4x4));

            // Matrix4x4 has 16 elements (4x4 matrix)
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    var element = value[row, col];
                    _floatProcessor.Process($"m{row}{col}", ref element, formatter);

                    if (formatter.Operation == FormatterOperation.Read)
                    {
                        value[row, col] = element;
                    }
                }
            }
        }
    }
}
