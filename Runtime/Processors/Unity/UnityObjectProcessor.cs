using System;
using EasyToolKit.Serialization.Formatters;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class UnityObjectProcessor<T> : SerializationProcessor<T>
        where T : UnityEngine.Object
    {
        public override void Process(string name, ref T value, IDataFormatter formatter)
        {
            UnityEngine.Object unityObject = value;

            formatter.BeginMember(name);
            formatter.Format(ref unityObject);

            if (formatter.Operation == FormatterOperation.Read)
            {
                value = unityObject as T;
            }
        }
    }
}
