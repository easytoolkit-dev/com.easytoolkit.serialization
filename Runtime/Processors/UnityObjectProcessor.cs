using System;

namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.UnityObject)]
    public class UnityObjectProcessor<T> : SerializationProcessor<T>
        where T : UnityEngine.Object
    {
        protected override void Process(string name, ref T value, IDataFormatter formatter)
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
