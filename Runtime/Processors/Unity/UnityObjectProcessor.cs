using System;
using EasyToolkit.Serialization.Formatters;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Unity)]
    public class UnityObjectProcessor<T> : SerializationProcessor<T>
        where T : UnityEngine.Object
    {
        protected override void Process(string name, ref T value, IDataFormatter formatter)
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
