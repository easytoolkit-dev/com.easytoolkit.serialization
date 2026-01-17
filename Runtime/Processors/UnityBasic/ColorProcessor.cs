using UnityEngine;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.UnityBasic)]
    public class ColorProcessor : SerializationProcessor<Color>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatSerializer;

        protected override void Process(string name, ref Color value, IDataFormatter formatter)
        {
            _floatSerializer.Process("R", ref value.r, formatter);
            _floatSerializer.Process("G", ref value.g, formatter);
            _floatSerializer.Process("B", ref value.b, formatter);
            _floatSerializer.Process("A", ref value.a, formatter);
        }
    }
}
