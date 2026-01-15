using UnityEngine;

namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.UnityBasic)]
    public class ColorSerializer : EasySerializer<Color>
    {
        private static readonly EasySerializer<float> FloatSerializer = GetSerializer<float>();

        public override void Process(string name, ref Color value, IDataFormatter formatter)
        {
            FloatSerializer.Process("R", ref value.r, formatter);
            FloatSerializer.Process("G", ref value.g, formatter);
            FloatSerializer.Process("B", ref value.b, formatter);
            FloatSerializer.Process("A", ref value.a, formatter);
        }
    }
}
