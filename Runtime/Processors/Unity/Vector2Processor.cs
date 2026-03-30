using EasyToolkit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class Vector2Processor : SerializationProcessor<Vector2>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatProcessor;

        protected override void Process(ref Vector2 value, IDataFormatter formatter)
        {
            using var scope = formatter.EnterObject(typeof(Vector2));

            var x = value.x;
            _floatProcessor.Process("X", ref x, formatter);
            var y = value.y;
            _floatProcessor.Process("Y", ref y, formatter);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = new Vector2(x, y);
            }
        }
    }
}
