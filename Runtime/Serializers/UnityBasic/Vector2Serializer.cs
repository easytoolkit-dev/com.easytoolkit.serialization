using UnityEngine;

namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.UnityBasic)]
    public class Vector2Serializer : EasySerializer<Vector2>
    {
        private static readonly EasySerializer<float> FloatSerializer;
        public override void Process(string name, ref Vector2 value, IDataFormatter formatter)
        {
            FloatSerializer.Process("x", ref value.x, formatter);
            FloatSerializer.Process("y", ref value.y, formatter);
        }
    }
}
