using UnityEngine;

namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.UnityBasic)]
    public class ColorProcessor : SerializationProcessor<Color>
    {
        private static readonly ISerializationProcessor<float> FloatSerializer;

        protected override void Process(string name, ref Color value, IDataFormatter formatter)
        {
            FloatSerializer.Process("R", ref value.r, formatter);
            FloatSerializer.Process("G", ref value.g, formatter);
            FloatSerializer.Process("B", ref value.b, formatter);
            FloatSerializer.Process("A", ref value.a, formatter);
        }
    }
}
