using UnityEngine;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.UnityBasic)]
    public class ColorProcessor : SerializationProcessor<Color>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatSerializer;

        public override void Process(string name, ref Color value, IDataFormatter formatter)
        {
            if (!IsRoot) formatter.BeginMember(name);
            using var objectScope = formatter.EnterObject();

            _floatSerializer.Process("r", ref value.r, formatter);
            _floatSerializer.Process("g", ref value.g, formatter);
            _floatSerializer.Process("b", ref value.b, formatter);
            _floatSerializer.Process("a", ref value.a, formatter);
        }
    }
}
