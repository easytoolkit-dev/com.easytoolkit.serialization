using UnityEngine;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.UnityBasic)]
    public class Vector3Processor : SerializationProcessor<Vector3>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatSerializer;

        protected override void Process(string name, ref Vector3 value, IDataFormatter formatter)
        {
            _floatSerializer.Process("x", ref value.x, formatter);
            _floatSerializer.Process("y", ref value.y, formatter);
            _floatSerializer.Process("z", ref value.z, formatter);
        }
    }
}
