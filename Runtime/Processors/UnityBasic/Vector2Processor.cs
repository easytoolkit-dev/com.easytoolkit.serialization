using UnityEngine;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.UnityBasic)]
    public class Vector2Processor : SerializationProcessor<Vector2>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatSerializer;

        protected override void Process(string name, ref Vector2 value, IDataFormatter formatter)
        {
            _floatSerializer.Process("x", ref value.x, formatter);
            _floatSerializer.Process("y", ref value.y, formatter);
        }
    }
}
