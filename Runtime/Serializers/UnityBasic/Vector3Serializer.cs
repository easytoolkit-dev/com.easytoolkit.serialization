using UnityEngine;

namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.UnityBasic)]
    public class Vector3Serializer : EasySerializer<Vector3>
    {
        private static readonly EasySerializer<float> FloatSerializer;

        public override void Process(string name, ref Vector3 value, IDataFormatter formatter)
        {
            FloatSerializer.Process("x", ref value.x, formatter);
            FloatSerializer.Process("y", ref value.y, formatter);
            FloatSerializer.Process("z", ref value.z, formatter);
        }
    }
}
