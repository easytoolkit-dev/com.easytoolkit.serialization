using System;

namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.UnityObject)]
    public class UnityObjectSerializer<T> : EasySerializer<T>
        where T : UnityEngine.Object
    {
        public override void Process(string name, ref T value, IDataFormatter formatter)
        {
            UnityEngine.Object unityObject = value;

            formatter.BeginMember(name);
            formatter.Format(ref unityObject);
            formatter.EndMember();

            if (formatter.Direction == FormatterDirection.Input)
            {
                value = unityObject as T;
            }
        }
    }
}
