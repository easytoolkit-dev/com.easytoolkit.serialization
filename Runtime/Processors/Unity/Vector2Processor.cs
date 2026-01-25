using EasyToolKit.Serialization.Formatters;
using UnityEngine;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.UnityBasic)]
    public class Vector2Processor : SerializationProcessor<Vector2>
    {
        [DependencyProcessor]
        private ISerializationProcessor<float> _floatSerializer;

        public override void Process(string name, ref Vector2 value, IDataFormatter formatter)
        {
            if (!IsRoot) formatter.BeginMember(name);
            using var objectScope = formatter.EnterObject();

            _floatSerializer.Process("x", ref value.x, formatter);
            _floatSerializer.Process("y", ref value.y, formatter);
        }
    }
}
