using UnityEngine;

namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.UnityBasic)]
    public class Vector2Processor : SerializationProcessor<Vector2>
    {
        private static readonly ISerializationProcessor<float> FloatSerializer;

        protected override void Process(string name, ref Vector2 value, IDataFormatter formatter)
        {
            FloatSerializer.Process("x", ref value.x, formatter);
            FloatSerializer.Process("y", ref value.y, formatter);
        }
    }
}
